import datetime
import time
import os
import sys
import math

import numpy
from sklearn.ensemble import RandomForestClassifier
from sklearn.neighbors import KNeighborsClassifier
from sklearn.externals import joblib

from scipy import signal
import pyeeg

logpath = "Log\\";
filepath = "Louis-JVI-log.txt"
facepath = "Louis-JVI-FaceReaderLog.txt"
decisionpath = "Louis-JVI-decision.txt"
easypath = "Louis-JVI-calibEasy.txt"
hardpath = "Louis-JVI-calibHard.txt"
eeg_path = "EEG.csv"

length = 2048 # Number of samples
order = 5 # Order of the filter
sample_rate = 128 # Hz, Emotiv specifications
cutoff_freq = 0.16 # Hz, recommendation by Emotiv
bands = [0.5,4,7,12,30] # Limits of bands of interests (in Hz too)

# Creating a filter and returning its coefficients  
nyquist = 0.5*sample_rate
cutoff = cutoff_freq / nyquist
b_coef, a_coef = signal.butter(order, cutoff, btype="high", analog = False)

# Creating a window function
hannwindow = numpy.hanning(length)

def classification(log, knnClassifier, rfClassifier, eeg_segment = None):
    if len(log) > 1:
        log_array = numpy.vstack(log)
        seq_array = log_array[:,1:-2]
        seq_array = seq_array[-3000:]
        seq_array = statisticalparameters(seq_array)
        if eeg_segment != None:
            seq_array = numpy.concatenate((seq_array,eeg_segment[:,-2]))
        prediction = knnClassifier.predict(seq_array)
        if rfClassifier != None:
            predictionRF = rfClassifier.predict(seq_array)
        else:
            predictionRF = [0]
        answer = str(prediction[0])+","+str(predictionRF[0])
        return answer
    else:
        return "0,0"

def create_sample(training_array, label_array):
    length = (training_array.shape[0]/20)-1
    samples = []
    labels = []
    for i in range(training_array.shape[0]-length-1):
        #print training_array[i:length]
        samples.append(statisticalparameters(training_array[i:i+length]))
        labels.append(label_array[i+length-1])

    print numpy.array(labels)
    return numpy.vstack(samples), numpy.array(labels)

def train(logall, easy_time, hard_time, eeg_log=False):
    rfClassifier = RandomForestClassifier(n_estimators=100)
    
    times = [easy_time, hard_time]
    times_seconds = []
    for time in times:
        time = time.strip().split(" ")
        times_seconds.append(time_to_seconds(time[1]))

    log_array = numpy.vstack(logall)

    indices = [0,0]
    time_array = log_array[:,-1]
    for i in range(2):
        for index,time in enumerate(time_array):
            if math.fabs(time - times_seconds[i]) < 0.01:
                indices[i] = index
                break
    
    easy_array = log_array[0:indices[0]]
    hard_array = log_array[indices[0]:indices[1]]

    labels0 = [0]*easy_array.shape[0]
    labels1 = [1]*hard_array.shape[0]

    print easy_array.shape, hard_array.shape
    
    label_array = numpy.array(labels0+labels1,dtype=numpy.int32)
    training_array = numpy.vstack([easy_array, hard_array])

    training_array = training_array[:,1:-2]
    training_array, label_array = create_sample(training_array, label_array)

    rfClassifier.fit(training_array, label_array)
    joblib.dump(rfClassifier,os.path.join(logpath,"rfClassifier.clf"))
    return rfClassifier

def statisticalparameters(sequence):
    return numpy.hstack([sequence.mean(axis=0),sequence.std(axis=0),sequence.min(axis=0),sequence.max(axis=0)])

def seconds_to_time(timeseconds):
    return str(datetime.timedelta(seconds=timeseconds))
 
def time_to_seconds(timestring):
    remainder = float(timestring.split('.')[1])/1000.0
    x = time.strptime(timestring.split('.')[0],'%H:%M:%S')
    return datetime.timedelta(hours=x.tm_hour,minutes=x.tm_min,seconds=x.tm_sec).total_seconds()+remainder

def data_imputation(log, segment):
    """ 0 if no signal has been detected yet, interpolated with nearest values otherwise """
    if numpy.isnan(segment).any():
        if len(log) > 1: 
            log_array = numpy.vstack(log)
            log_and_segment = numpy.vstack((log_array,segment))
        else:
            log_and_segment = segment
        for i in range(1,segment.shape[1]-1):
            col = log_and_segment[:,i]
            if numpy.isnan(col).all():
                segment[:,i] = numpy.nan_to_num(col)
            elif numpy.isnan(col).any():
                mask = numpy.isnan(col)
                col[mask] = numpy.interp(numpy.flatnonzero(mask), numpy.flatnonzero(~mask), col[~mask])
                segment[:,i] = col[0:segment.shape[0]]
        return segment
    else:
        return segment

def text_to_array(log, segment, face_segment, eeg_segment=None):
    lines = [line.strip().split(" ") for line in segment]
    for line in lines:
        line[-1] = time_to_seconds(line[-1])
    
    segment = numpy.array(lines)
    
    segment[segment == "FIT_FAILED"] = numpy.nan
    segment[segment == "FIND_FAILED"] = numpy.nan
    segment[segment == "-1"] = numpy.nan
    segment[0][1] = numpy.nan
    
    segment = segment.astype(numpy.float64)
    segment = segment[:,4:]

    # Face synchronization and stacking

    #lines = [line for line in face_segment]
    for line in face_segment:
        line[0] = time_to_seconds(line[0])

    face_segment = numpy.array(face_segment).astype(numpy.float64)

    to_stack = numpy.zeros((segment.shape[0],face_segment.shape[1]))
    to_stack[to_stack == 0] = numpy.nan
    
    times = segment[:,-1]
    for row in face_segment:
        current_time = row[0]
        smallest = float("inf")
        new_distance = 0
        for i,one_time in enumerate(times):
            new_distance = math.fabs(one_time - current_time)
            if new_distance > smallest:
                to_stack[i-1] = row
                break
            else:
                smallest = new_distance

    #print segment.shape
    #print "Stacking!"
    segment = numpy.hstack((to_stack,segment))
    #print segment.shape

    # Eeg synchronization and stacking

    #segment = numpy.hstack((eeg_to_stack, segment))

    segment = data_imputation(log, segment)

    return segment

def process_eeg(segment): # untested!
    # Group each channel in a sequence then run the processing on each sequence, missing the formatting for that right now
    
    segment = [line.strip().split(" , ") for line in segment]

    names = segment[0]

    segment = segment[1:]

    segment = numpy.array(segment)

    channels = [onetime[:,i] for i in range(segment.shape[1])]

    features = []

    for channel in channels[1:15]:
        channel = signal.lfilter(b_coef, a_coef, channel)
        windowed_seq = channel*hannwindow
        power, power_ratios = pyeeg.bin_power(windowed_seq, bands, sample_rate)
        feature.append(numpy.concatenate((power, power_ratios)))

    features = numpy.concatenate(features)
    relativetimestamp = numpy.mean(channels[:,17])       
    systemtime = channels[:,-1][-1]
    eegfeatures = numpy.concatenate((features, relativetimestamp, systemtime))
    return eegfeatures

def process_facereader(segment):
    segment = [line.replace(",",".").strip().split("    ") for line in segment]
    #print len(segment)
    final_segment = []
    for line in segment:
        if len(line) > 10:
            line = line[1:11]
            final_line = [value.strip().split(" ")[-1] for value in line]
            final_segment.append(final_line)
        else:
            final_line = [line[1].strip().split(" ")[-1]]+[numpy.nan, numpy.nan, numpy.nan, numpy.nan, numpy.nan, numpy.nan, numpy.nan, numpy.nan, numpy.nan]
            final_segment.append(final_line)

    return final_segment

def write_back(decision,time):
    with open(os.path.join(logpath,decisionpath),"a") as opened_file:
        opened_file.write(decision+","+str(time)+"\n")

def main():
    knnClassifier = joblib.load("knn7.clf")
    rfClassifier = None

    trained = False

    log = []
    easy_time = 0
    hard_time = 0
    eeg_log = []
    print "Ready to start"
    while filepath not in os.listdir(logpath):
        time.sleep(5)
    with open(os.path.join(logpath,filepath),"r") as opened_file:
        with open(os.path.join(logpath,facepath),"r") as face_file:
            while True:
                segment = opened_file.readlines()
                face_segment = face_file.readlines()
                eeg_segment = eeg_file.readlines()
                if len(face_segment) > 1:
                    face_segment = process_facereader(face_segment)
                if len(eeg_segment) > 1:
                    eeg_segment = process_eeg(eeg_segment)
                    eeg_log.append(eeg_segment)
                if len(segment) > 1:
                    #print segment[0], segment[-1]
                    segment = text_to_array(log,segment,face_segment, eeg_segment)
                    log.append(segment)
                    print segment.shape, segment[0][-1], segment[-1][-1], segment.dtype
                    decision = classification(log, knnClassifier, rfClassifier, eeg_segment)
                    write_back(decision,segment[-1][-1])
                elif len(log) > 1:
                    print "Log saved, ready to quit"
                    numpy.save(os.path.join(logpath,"currentparticipant"),numpy.vstack(log))

                if (hardpath in os.listdir(logpath)) and not trained:
                    with open(os.path.join(logpath,easypath),"r") as opened_easy:
                        easy_time = opened_easy.read()
                    with open(os.path.join(logpath,hardpath),"r") as opened_hard:
                        hard_time = opened_hard.read()
                    rfClassifier = train(log, easy_time, hard_time, eeg_log)
                    trained = True

                time.sleep(20)


main()

"""

Checklist

Start FaceReader
Start FaceReader External Control
Connect to FaceReader
Click Enable Detailed Log
Click Start Analysis

Start Emotiv Control Panel

Start TestBench

"""
#numpy.savetxt("currentparticipant.txt",a,fmt="%.6e")