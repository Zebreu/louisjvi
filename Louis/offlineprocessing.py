import os
import math
import datetime
import time
from sklearn.externals import joblib
 
from scipy.interpolate import interp1d
from scipy import interp
from scipy import fftpack
import scipy.stats
import numpy
import random
 
import matplotlib.pyplot as plt
 
from sklearn.neighbors import DistanceMetric
from sklearn.neighbors import KNeighborsClassifier
from sklearn.neighbors import RadiusNeighborsClassifier
from sklearn.neighbors import NearestCentroid
from sklearn.ensemble import RandomForestRegressor
from sklearn.ensemble import RandomForestClassifier
from sklearn.ensemble import GradientBoostingClassifier
from sklearn.ensemble import AdaBoostClassifier
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import AdaBoostRegressor
from sklearn.svm import SVR
from sklearn.svm import SVC
from sklearn.svm import LinearSVC
from sklearn.linear_model import LogisticRegression
 
from sklearn import decomposition
from sklearn import preprocessing
from sklearn import cross_validation
from sklearn import metrics
 
#filepathDecember = "/media/zebreu/TransferedFromUbuntu/10Dec1"
filepathDecember = "D:\\Data-Ramla-Seb-10Dec2014\\10DecUpdated\\10Dec"
#save_filepath = "/media/zebreu/TransferedFromUbuntu/10Dec1/arrays"
logpath = "Louis-JVI-log.txt"
prepath = "Louis-JVI-pretest.txt"
postpath = "Louis-JVI-posttest.txt"
personalitypath = "Personality.txt"
 
personalitypathApril = "BF.txt"

#filepathApril = "/media/zebreu/TransferedFromUbuntu/AprilModified/AprilExperiment"
filepathApril = "D:\\Data-Ramla-Seb-April2015\\aprilexperimentupdated\\aprilexperiment\\AprilModified\\AprilExperiment"

def remove_participants(labels,to_remove):
    
    index_to_remove = []
    for i,row in enumerate(labels):
        if row[0] in to_remove:
            index_to_remove.append(i)

    return numpy.delete(labels,index_to_remove,0)

def return_self_report_value(selflabels,pnumber,task):
    if task == 3:
        task = 4
    for row in selflabels:
        if row[0] == pnumber:
            return row[task+1]
    print "Could not find pnumber"
    return 99

def self_report_labelling(dataDec, dataApril):
    selflabelsDec = numpy.load("SelfReportLabelsDecember.npy")
    selflabelsApril = numpy.load("SelfReportLabelsApril.npy")
    
    selflabelsApril = selflabelsApril[:,:-1]

    selflabelsApril = remove_participants(selflabelsApril,[8,12])
    selflabelsDec = remove_participants(selflabelsDec,[10,17])

    print dataApril[0:30,-5:]
    return

    print selflabelsApril
    print selflabelsDec 

    sequence_labels  = []

    datapairs = [(selflabelsDec,dataDec),(selflabelsApril,dataApril)]
    for datapair in datapairs:
        selflabels = datapair[0]
        pnumbers = datapair[1][:,-5].astype(numpy.int32)
        tasks = datapair[1][:,-3].astype(numpy.int32)
        
        newlabels = numpy.array(tasks)

        for row in range(newlabels.shape[0]):
            newlabels[row] = return_self_report_value(selflabels,pnumbers[row],tasks[row])

        sequence_labels.append(newlabels)

    print sequence_labels[0][0:25]
    print dataDec[0:30]



def parse_questionnaire(filepath=filepathDecember):
    """ April P8?, P10, P18(no file) did not fill questionnaire
        December P5 did not either
    """
    pnumbers = []
    questionnaires = []
    directories = sorted(os.listdir(filepath))
    for directory in directories:
        if "P" in directory:
            for filename in os.listdir(os.path.join(filepath,directory)):
                if "tionnaire" in filename:
                    with open(os.path.join(filepath,directory,filename)) as opened_file:
                        questionnaires.append(opened_file.readlines())
                        pnumbers.append(int(directory[1:]))
    
    labels = []
    final_labels = []
    for i,questionnaire in enumerate(questionnaires):
        counter = 0
        print "P",pnumbers[i]
        labels.append([str(pnumbers[i])])
        for line in questionnaire:
            if ":" in line:
                counter += 1
                if filepath==filepathApril:
                    if counter < 7:
                        label = line.strip()[-1]
                        if label == "." or label == ":":
                            pass
                        else:
                            labels[-1].append(label)
                else:
                    if counter < 7 and counter > 1:
                        label = line.strip()[-1]
                        if label == "." or label == ":":
                            pass
                        else:
                            labels[-1].append(label)
        if len(labels[-1]) == 6 and filepath==filepathDecember:
            final_labels.append(labels[-1]) 
        elif filepath==filepathApril:
            last = labels[-1][-1]
            labels[-1].pop(-1)
            while len(labels[-1]) < 6:
                labels[-1].append("3")
            labels[-1].append(last)
            final_labels.append(labels[-1])

    #print labels
    if filepath==filepathApril:
        final_labels.pop(9)
        final_labels.pop(7)
    #print labels
    print final_labels

    for p in final_labels:
        print len(p)

    labels = numpy.array(final_labels,dtype=numpy.int32)
    print labels.shape
    print labels

    #print scipy.stats.itemfreq(labels[:,1:6])
    #numpy.save("SelfReportLabelsDecember",labels)

def seconds_to_time(timeseconds):
    return str(datetime.timedelta(seconds=timeseconds))
 
def time_to_seconds(timestring):
    remainder = float(timestring.split('.')[1])/1000.0
    x = time.strptime(timestring.split('.')[0],'%H:%M:%S')
    return datetime.timedelta(hours=x.tm_hour,minutes=x.tm_min,seconds=x.tm_sec).total_seconds()+remainder
 
def divide_log(log):
    indices = []
    state = 0
    for i,line in enumerate(log):
        if line.split()[10] == state:
            pass
        else:
            state = line.split()[10]
            indices.append(i)
 
    #print indices
    #print len(indices)
    return indices
 
def get_sequences(log, indices):
    temp_log = []
    for line in log:
        temp_log.append(line.split(" "))

    logarray = numpy.array(temp_log,dtype="object")
    
    labels = numpy.zeros(len(logarray))
    sequences = []
    i = 0
    start = 0
    while i < len(indices)-1:
        current = logarray[indices[i+1]][10]
         
        if current != "0":
            stop = indices[i+1] 
            labels[start:stop] = current
            sequences.append((start,stop))
            start = stop
         
        i += 1
     
    #print sequences
    #print indices
     
    return sequences, labels, logarray
 
def detect_task(data):
    missions = []
    misssion_counter = 0
    participant_counter = 0
    for i in range(data.shape[0]):
        if data[i][-2] != participant_counter:
            mission_counter = 0
            participant_counter = data[i][-2]
        missions.append(mission_counter)
        if data[i][-1] == 2:
            mission_counter += 1
 
    return missions
 
    #missions = numpy.array(missions).reshape(158,1)
     
    #data = numpy.hstack((data,missions))
 
def calculate_duration(data, timing):
    #timing = timing[0:85]+timing[115:]
    #print timing
    durations = []
    start = []
    for t in timing:
        durations.append(t[1]-t[0])
        start.append(seconds_to_time(t[0]))

    return start, durations
 
def read_logs(filepath=filepathApril):
    pnumbers = []
    all_logs = []
 
    directories = sorted(os.listdir(filepath))
    for directory in directories:
        if "P" in directory:# and "ignore" not in directory:
            print directory
            with open(os.path.join(filepath,directory,logpath)) as opened_file:
                all_logs.append(opened_file.readlines())
                pnumbers.append(int(directory[1:]))
     
    all_indices = []
    for log in all_logs:
        all_indices.append(divide_log(log))
 
    no_affectiv = []
    for i,log in enumerate(all_logs):
        middle = len(log)/2
        if ["0","0","0","0","0"] == log[middle].split()[5:10]:
            no_affectiv.append(i)
 
    print no_affectiv #[1, 2, 7, 8, 9, 13, 14, 15, 19] = P10, P11, P16, P17, P18, P3, P4, P5, P9
    #P04 and P18 for aprilDataset
 
    seq_labels_log = []
    for log,indices in zip(all_logs,all_indices):
        seq_labels_log.append(get_sequences(log,indices))
 
    print len(seq_labels_log)
 
    master_list = []
    numbers = []
    number = 0
    timing = []
    for i,current in enumerate(seq_labels_log):
        logarray = current[2]
        labels = current[1]
        sequences = current[0]
        if True:
        #if i not in no_affectiv:
            print i
            #print all_indices[i]
            for sequence in sequences:
                if i == 15:
                    print logarray[0]

                seq_array = logarray[sequence[0]:sequence[1],4:10].astype(numpy.float32)
                seq_array[seq_array == -1] = numpy.nan
                seq_array = numpy.ma.masked_array(seq_array,numpy.isnan(seq_array))
                sample = numpy.hstack([seq_array.mean(0).filled(numpy.nan), seq_array.std(0).filled(numpy.nan), seq_array.max(0).filled(numpy.nan), seq_array.min(0).filled(numpy.nan),numpy.array(pnumbers[i]),numpy.array(labels[sequence[0]])])
                #print logarray[sequence[0]][-2], logarray[sequence[1]][-2]
                
                #if filepath == filepathApril:
                timing.append((time_to_seconds(logarray[sequence[0]][-2]),time_to_seconds(logarray[sequence[1]][-2])))
                #elif i != 11:
                #    timing.append((time_to_seconds(logarray[sequence[0]][-2]),time_to_seconds(logarray[sequence[1]][-2])))
                
                #sample = numpy.hstack([seq_array.mean(0), seq_array.std(0), seq_array.max(0), seq_array.min(0),numpy.array(labels[sequence[0]])])
                master_list.append(sample)
                number+=1
            numbers.append(number)
 
    num_array = numpy.array(numbers)
    data_array = numpy.array(master_list)
    
    tasks = detect_task(data_array)
 
    print tasks
    print len(tasks)
    #return
    tasks=numpy.array(tasks)

    durations, starts = calculate_duration(data_array, timing)

    durations = numpy.array(durations)
    starts =  numpy.array(starts)

    tasks = fix_shape(tasks)
    durations = fix_shape(durations)
    starts = fix_shape(starts)

    #print starts

    print data_array.shape
    data = numpy.hstack((data_array,tasks,durations,starts))
    #print data[100:200,-5]

    # Remove data from two April participants with messed up data : 8 and 12
    
    if filepath == filepathApril:
        to_remove = []
        for removable,line in enumerate(data):
            if int(float(line[-5])) == 8:
                to_remove.append(removable)
        print to_remove

        data = numpy.vstack((data[0:to_remove[0]],data[to_remove[-1]+1:]))

        to_remove = []
        for removable,line in enumerate(data):
            if int(float(line[-5])) == 12:
                to_remove.append(removable)
        print to_remove

        data = numpy.vstack((data[0:to_remove[0]],data[to_remove[-1]+1:]))

    #for_more_stats = data[:,-5:]
    #numpy.save("april",for_more_stats)
    
    # Remove data from two December participants with messed up data : 10 and 17
    
    
    if filepath == filepathDecember:
        to_remove = []
        for removable,line in enumerate(data):
            if int(float(line[-5])) == 10:
                to_remove.append(removable)
        print to_remove

        data = numpy.vstack((data[0:to_remove[0]],data[to_remove[-1]+1:]))

        to_remove = []
        for removable,line in enumerate(data):
            if int(float(line[-5])) == 17:
                to_remove.append(removable)
        print to_remove

        data = numpy.vstack((data[0:to_remove[0]],data[to_remove[-1]+1:]))

    print max(data[:,-3].tolist())
    for_more_stats = data[:,-5:]
    #numpy.save("april",for_more_stats)
    #print for_more_stats[-3].max()
    

    #numpy.savetxt("decemberParticipants.txt",for_more_stats,fmt="%s")

    #calculate_game_stats(for_more_stats)

    return data

def calculate_game_stats(data):
    histogram = [0]*6
    for line in data:
        histogram[int(line[-3])] += 1

    plt.bar(range(6),histogram)
    plt.show()

    total_durations = [0]*6
    durations = [[]]*6
    for line in data:
        total_durations[int(line[-3])] += float(line[-1])
        durations[int(line[-3])].append(float(line[-1]))

    mean_durations = [total_durations[i]/histogram[i] for i in range(6)]

    plt.bar(range(6),mean_durations)

    plt.show()

def fix_shape(vector):
    return vector.reshape(vector.shape[0],1)
 
def add_facereader():
    face_analysis = parse_facereader()
 
    face_list = []
 
    timing_numbers = [11,26,34,85,115,126,145,154,184,188]
    i = 0
    current_face = face_analysis.pop(0)
    start = 0
    face_index = 0
    for sample_timing in timing:
        print sample_timing
        if i == timing_numbers[0]:
            timing_numbers.pop(0)
            current_face = face_analysis.pop(0)
            start = 0
            face_index = 0
            print "New face"
        #print len(current_face)
        print current_face[face_index][0]
        while current_face[face_index][0] < sample_timing[0]:
            face_index += 1
        start = face_index
        while current_face[face_index][0] < sample_timing[1]:
            face_index += 1
        stop = face_index
 
        print (start,stop)
        face_array = numpy.array(current_face[start:stop])
        face_array = face_array[:,1:]
        face_array[face_array == "FIT_FAILED"] = numpy.nan
        face_array[face_array == "FIND_FAILED"] = numpy.nan
        face_array = face_array.astype(numpy.float32)
        face_array = numpy.ma.masked_array(face_array,numpy.isnan(face_array))
        face_list.append(numpy.hstack([face_array.mean(0).filled(numpy.nan), face_array.std(0).filled(numpy.nan), face_array.max(0).filled(numpy.nan), face_array.min(0).filled(numpy.nan)]))
 
 
        i += 1
 
    #numpy.save("PFaceReaderData",numpy.array(face_list,dtype=numpy.float32))
 
    #numpy.save("PEyeAffectivData", data_array)
    #numpy.save("SamplesDivision", num_array)

def mean_imputation():
    data = read_logs()
    decdata = read_logs(filepathDecember)

    alldata = numpy.vstack((decdata,data))

    #print decdata[0]
    #print data[0]
    #print data[4]
    #print data[100]

    alldata = alldata.astype(numpy.float32)

    for row in range(alldata.shape[0]):
        for column in [0,6,12,18]:
            if not numpy.isnan(alldata[row][column]):
                if alldata[row][column] == 0:
                    alldata[row][column] = numpy.nan

    print numpy.nanmean(alldata,0)

def preprocess():
    dataApril = read_logs()
    dataDec = read_logs(filepathDecember)

    self_report_labelling(dataDec,dataApril)


if __name__ == "__main__":
    #read_logs()
    #parse_questionnaire()
    #mean_imputation()
    preprocess()