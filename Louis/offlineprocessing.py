import sys
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

import pyeeg
 
#filepathDecember = "/media/zebreu/TransferedFromUbuntu/10Dec1"
filepathDecember = "E:\\TransferedFromUbuntu\\10Dec1"
#filepathDecember = "D:\\Data-Ramla-Seb-10Dec2014\\10DecUpdated\\10Dec"
#filepathDecember = "C:\\Users\\locarno\\Documents\\backupzephyr\\10Dec"
#save_filepath = "/media/zebreu/TransferedFromUbuntu/10Dec1/arrays"
logpath = "Louis-JVI-log.txt"
prepath = "Louis-JVI-pretest.txt"
postpath = "Louis-JVI-posttest.txt"
personalitypath = "Personality.txt"

facereaderpath = "Louis-JVI-FaceReaderLog.txt" 
personalitypathApril = "BF.txt"

#filepathApril = "/media/zebreu/TransferedFromUbuntu/AprilModified/AprilExperiment"
filepathApril = "E:\\TransferedFromUbuntu\\AprilModified\\AprilExperiment"
#filepathApril = "D:\\Data-Ramla-Seb-April2015\\aprilexperimentupdated\\aprilexperiment\\AprilModified\\AprilExperiment"
#filepathApril = "C:\\Users\\locarno\\Documents\\backupzephyr\\April\\uptodateAprilExp\\AprilModified\\AprilExperiment"

def remove_participants(to_remove,labels=False, data=False):
    
    index_to_remove = []
    if data != False:
        for i, row in enumerate(data):
            if int(float(row[-6])) in to_remove:
                index_to_remove.append(i)
        print index_to_remove
        return numpy.delete(data,index_to_remove,0)

    else:
        for i,row in enumerate(labels):
            if row[0] in to_remove:
                index_to_remove.append(i)

        return numpy.delete(labels,index_to_remove,0)


def return_self_report_value(selflabels,pnumber,task):
    pnumber = int(float(pnumber))
    if task > 3:
        task += -1
    for row in selflabels:
        if row[0] == pnumber:
            return row[task+1]
    print "Could not find pnumber", pnumber
    return 99

def self_report_labelling(dataDec, dataApril):
    selflabelsDec = numpy.load("SelfReportLabelsDecember.npy")
    selflabelsApril = numpy.load("SelfReportLabelsApril.npy")
    
    selflabelsApril = selflabelsApril[:,:-1]

    selflabelsApril = remove_participants([8,12],labels=selflabelsApril)
    selflabelsDec = remove_participants([10,17],labels=selflabelsDec)

    selflabelsApril[:,0] += 20
    print dataApril[0:30,-5:]

    print selflabelsApril
    print selflabelsDec 

    sequence_labels  = []

    datapairs = [(selflabelsDec,dataDec),(selflabelsApril,dataApril)]
    for datapair in datapairs:
        selflabels = datapair[0]
        pnumbers = datapair[1][:,-5]#.astype(numpy.int32)
        tasks = datapair[1][:,-3].astype(numpy.int32)
        
        newlabels = numpy.array(tasks)

        for row in range(newlabels.shape[0]):
            newlabels[row] = return_self_report_value(selflabels,pnumbers[row],tasks[row])

        sequence_labels.append(newlabels)

    #print sequence_labels[0][0:25]
    #print dataDec[0:30]

    return sequence_labels

def parse_personality(filepath=filepathDecember):
    big_five = []
    directories = sorted(os.listdir(filepath))
    for directory in directories:
        if "P" in directory:
            print directory
            if filepath == filepathApril:
                personalitypath = "BF.txt"
            else:
                personalitypath = "Personality.txt"
            if os.path.isfile(os.path.join(filepath,directory,personalitypath)):
                with open(os.path.join(filepath,directory,personalitypath)) as opened_file:    
                    current = []
                    file_string = opened_file.read()
                    start = 0
                    while len(current) < 5:
                        position = file_string.find("dimension (",start)+len("dimension (")
                        print file_string[position:position+2]
                        current.append(int(file_string[position:position+2]))
                        start = position+2
            else:
                current = [-1,-1,-1,-1,-1]   
            directory = int(directory[1:]) 
            current.append(directory)
            big_five.append(current)

    #print big_five
    bf = numpy.array(big_five).astype(numpy.float32)
    return bf

def preprocess_bigfive():
    bfDec = parse_personality(filepath=filepathDecember)
    bfApril = parse_personality(filepath=filepathApril)
    bfApril[:,-1] += 20

    allbf = numpy.vstack((bfDec, bfApril)).astype(numpy.float32)

    allbf[allbf == -1] = numpy.nan
    means = numpy.nanmean(allbf,1)

    for row in allbf:
        if row[-1] == 33:
            row[0:5] = means[0:5]

    numpy.save("bfarray",allbf)

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
    """ Returns when the participant started a new task """
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

def read_logs_and_save_all(filepath=filepathApril):
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
 
    seq_labels_log = []
    for log,indices in zip(all_logs,all_indices):
        seq_labels_log.append(get_sequences(log,indices))
    
    """
    totallines = 0
    print len(seq_labels_log)
    for seq in seq_labels_log:
        totallines += seq[2].shape[0]
    
    print totallines
    """
    print pnumbers
    if filepath == filepathApril:
        removings = [8,10,12,18]
    else:
        removings = [5,10,17]
    for removing in removings:
        for index,pnumber in enumerate(pnumbers):
            print index,pnumber
            if pnumber == removing:
                seq_labels_log.pop(index)
                print index
                break
        pnumbers.pop(index)             

    master_list = []

    number = 0 
    for i,current in enumerate(seq_labels_log):
        logarray = current[2]
        labels = current[1]
        sequences = current[0]
        if True:
        #if i not in no_affectiv:
            print i
            #print all_indices[i]
            for sequence in sequences:
                print number
                #if i == 15:
                #    print logarray[0]
                
                seq_array = logarray[sequence[0]:sequence[1],4:10].astype(numpy.float32)
                more_timings = logarray[sequence[0]:sequence[1],-2]
                seq_array[seq_array == -1] = numpy.nan
                for timing_index, seq_row in enumerate(seq_array):
                    master_list.append(numpy.hstack((time_to_seconds(more_timings[timing_index]),seq_row, pnumbers[i], labels[sequence[0]],number)))
                    
                #print master_array[0]
                #print master_array[1]
                number += 1

    master_array = numpy.array(master_list,dtype=numpy.float32)

    numpy.save(os.path.join(filepathApril,"sequentialDataDecember"),master_array)


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

    # Order of stats: Pnumber, SuccessOrNot, TaskNumber, Time, Duration

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

def add_facereader(filepath):
    # Order of features = ["Neutral", "Happy", "Sad", "Angry","Surprised","Scared","Disgusted", "Valence","Arousal" ]
    
    allfacereader = []
    pnumbers = []

    if filepath == filepathApril:
        
        directories = sorted(os.listdir(filepath))
        for directory in directories:
            if "P" in directory:# and "ignore" not in directory:
                print directory
                pnumbers.append(directory)
                if os.path.isfile(os.path.join(filepath,directory,facereaderpath)):
                    with open(os.path.join(filepath,directory,facereaderpath)) as opened_file:
                        allfacereader.append(opened_file.readlines())
                else:
                    allfacereader.append([])

    #print line[0].strip().split(" ")[-1]

    totallines = 0
    for participant in allfacereader:
        totallines += len(participant)

    facereaderarray = numpy.zeros((totallines,11),dtype=numpy.float32)
    facereaderarray += -1

    row = 0
    for participant,pnumber in zip(allfacereader,pnumbers):
        pnumber = float(int(pnumber[1:]))
        print pnumber
        for line in participant:
            line = line.split("    ")[1:11]
            values = [item.strip().split(" ")[-1] for item in line]
            if len(values) < 10:
                values.pop(-1)
                values.extend([-1]*9)
            values[0] = time_to_seconds(values[0])

            for col in range(10):
                value = values[col]
                if isinstance(value,basestring):
                    value = value.replace(",",".")
                facereaderarray[row][col] = value
        
                
            facereaderarray[row][-1] = pnumber
            
            row += 1


    print facereaderarray[0:10]
    print facereaderarray[-10:]

    numpy.save("allfacereader",facereaderarray)

def dec_parse_facereader(filepath=filepathDecember):
    allfacereader = []
    pnumbers = []
    directories = sorted(os.listdir(os.path.join(filepath,"FaceReaderAnalyses2")))
    print directories
    for filename in directories:
        if "detailed" in filename:
            print filename
            with open(os.path.join(filepath,"FaceReaderAnalyses2",filename)) as opened_file:    
                allfacereader.append(opened_file.readlines())
            pnumbers.append(int(filename.split(" ")[1].split("_")[0]))

    print pnumbers
    alltimes = ["16:35:58.000","10:28:06.000","13:26:11.000","15:07:32.000","17:25:11.000","10:19:41.000","15:07:36.000","17:31:38.000","15:37:35.000","14:20:56.000","16:37:48.000","13:25:15.000","10:27:26.000","13:13:00.000","15:15:17.000","10:53:14.000"]
    
    # P11: 4:36:10, 
    # P1: "3:37:35" ,P12: "10:28:06", P13: "1:26:11", P14: "3:12:44",P15: "5:25:11",P19: 5:31:38,P20: 2:20:56,P6: 10:27:26,P7: 1:13:00, P8: 3:15:17
    #alltimes = ["15:37:35.000","10:28:06.000","13:26:11.000","15:12:44.000","17:25:11.000","17:31:38.000","14:20:56.000","10:27:26.000","13:13:00.000","15:15:17.000"]

    cut_analyses = []
    for analysis in allfacereader:
        analysis = analysis[10:]
        analysis2 = []
        system_time = time_to_seconds(alltimes.pop(0))
        for line in analysis:
            line = line.split()[0:10]
            line[0] = time_to_seconds(line[0])+system_time
            #print line
            analysis2.append(line)

        cut_analyses.append(analysis2)    

    master_list = []
    for i,participant in enumerate(cut_analyses):
        print pnumbers[i]
        for row in participant:
            row = numpy.array(row)
            row[row == "FIT_FAILED"] = -1.0
            row[row == "FIND_FAILED"] = -1.0
            master_list.append(numpy.hstack((row,pnumbers[i])))
    
    master_array = numpy.array(master_list,dtype=numpy.float32)
    numpy.save("allfacereaderDecember",master_array)

    print master_array[0]
    print master_array[-1]
    print master_array[10:20]

def mean_imputation(alldata):
    
    alldata = alldata.astype(numpy.float32)

    for row in range(alldata.shape[0]):
        for column in [0,6,12,18]:
            if not numpy.isnan(alldata[row][column]):
                if alldata[row][column] == 0:
                    alldata[row][column] = numpy.nan

    means = numpy.nanmean(alldata,0)

    for row in alldata:
        for i, value in enumerate(row):
            if numpy.isnan(value):
                row[i] = means[i]

    return alldata

def preprocess():
    dataApril = read_logs()
    dataDec = read_logs(filepathDecember)
    tempArray = dataApril[:,-5].astype(numpy.float32)
    tempArray += 20
    dataApril[:,-5] = tempArray
    print dataApril[:,-5]
    
    labels = self_report_labelling(dataDec,dataApril)
    #print len(labels[0]), len(dataDec), len(labels[1]), len(dataApril) 

    dataDec = numpy.hstack((dataDec,labels[0].reshape(len(labels[0]),1)))
    dataApril = numpy.hstack((dataApril,labels[1].reshape(len(labels[1]),1)))

    dataDec = remove_participants([5],data=dataDec)
    dataApril = remove_participants([30,38],data=dataApril)



    alldata = numpy.vstack((dataDec,dataApril))

    game_info = alldata[:,-6:]

    features = mean_imputation(alldata[:,0:24])
    
    alldata = numpy.hstack((features,game_info))

    numpy.save("allLogData2experiments",alldata) #0-312 exp.1, 313-632 exp.2

def stack_facereader(alldata, facereaderApril, facereaderDecember):
    facereaderApril[:,-1] += 20
    allfacereader = numpy.vstack((facereaderDecember,facereaderApril))

    allfacereader[allfacereader == -1] = numpy.nan

    master_list = []
    for i,row in enumerate(alldata):
        start = time_to_seconds(row[-3])
        duration = float(row[-2])
        pnumber = float(row[-6])
        stop = start+duration

        frslice = allfacereader[allfacereader[:,-1] == pnumber]
        
        frslice = frslice[frslice[:,0] > start]
        frslice = frslice[frslice[:,0] < stop]
        #print frslice.shape, i, pnumber, start, stop
        if frslice.shape[0] == 0:
            moredata = numpy.zeros((36))
            moredata[moredata == 0] = numpy.nan
        else:
            features = frslice[:,1:-1]
            moredata = numpy.hstack((numpy.nanmean(features,0), numpy.nanstd(features,0), numpy.max(features,0), numpy.min(features,0)))
            
        master_list.append(moredata)

    facearray = numpy.array(master_list,dtype=numpy.float32)

    means = numpy.nanmean(facearray,0)
    print means
    for row in facearray:
        for i, value in enumerate(row):
            if numpy.isnan(value):
                row[i] = means[i]

    print facearray.shape
    return numpy.hstack((facearray,alldata))

def stack_big_five(alldata,bf):
    adding = numpy.zeros((alldata.shape[0],5),dtype=numpy.float32)
    alldata = numpy.hstack((adding,alldata))
    for row in alldata:
        pnumber = row[-6]
        sliced = bf[bf[:,-1] == float(pnumber)]
        row[0:5] = sliced[0][0:5]
    
    return alldata

def classify_samples(generate_files=False):
    if generate_files:
        alldata = numpy.load("allLogData2experiments.npy")

        facereaderApril = numpy.load("allfacereaderApril.npy")
        facereaderDecember = numpy.load("allfacereaderDecember.npy")

        alldata = stack_facereader(alldata, facereaderApril, facereaderDecember)

        #numpy.save("633samples60features",alldata)
    else:
        alldata = numpy.load("633samples60features.npy")

    scaler = preprocessing.MinMaxScaler((-1,1))
    bfarray = numpy.load("bfarray.npy")
    #bfarrayFeatures = preprocessing.scale(bfarray[:,0:-1].astype(numpy.float32),axis=0)
    bfarrayFeatures = scaler.fit_transform(bfarray[:,0:-1].astype(numpy.float32))
    bfarray = numpy.hstack((bfarrayFeatures,bfarray[:,-1:]))
    
    
    scaler = preprocessing.MinMaxScaler((-1,1))
    #features = preprocessing.scale(alldata[:,0:-6].astype(numpy.float32),axis=0)  
    features = scaler.fit_transform(alldata[:,0:-6].astype(numpy.float32))
    alldata = numpy.hstack((features,alldata[:,-6:]))
    
    #alldata = stack_big_five(alldata, bfarray)

    grid_search(alldata)

    #leave_one_participant_out(alldata,1.0,0.001) scaled
    #leave_one_participant_out(alldata,10.0,0.001) normalized all, 53.4
    #leave_one_participant_out(alldata,10.0,1.0) # no facereader, 53.8
    #leave_one_participant_out(alldata,100.0,0.01) # no eye, 53.01
    #leave_one_participant_out(alldata,0.01,1000) # no emotiv, 46.05
    #leave_one_participant_out(alldata,100.0,0.01) # no eye, 53.01
    #leave_one_participant_out(alldata,0.01,100) # only facereader, 45
    #leave_one_participant_out(alldata,0.001,1000) # only eye, 46
    #leave_one_participant_out(alldata,10,0.001) # only emotiv, 54.1
    #leave_one_participant_out(alldata,1.0,1.0)

def extract_features(frsliced, sliced, info):
    
    if frsliced.shape[0] == 0:
        frsample = numpy.mean(info[:,0:36].astype(numpy.float32),0)
    else:
        features = frsliced
        frsample = numpy.hstack((numpy.nanmean(features,0), numpy.nanstd(features,0), numpy.max(features,0), numpy.min(features,0)))
    
    features = sliced
    #print features.shape
    sample =  numpy.hstack((numpy.nanmean(features,0), numpy.nanstd(features,0), numpy.max(features,0), numpy.min(features,0)))
    
    return numpy.hstack((frsample,sample))
    
#weirdP4

def slicing(sequence, start, stop):
    sliced = sequence[sequence[:,0] > start]
    sliced = sliced[sliced[:,0] < stop]
    return sliced
#overlap=30, 61.6_69.5, 10.0, 0.001
def create_samples(info, array, facereader, length=60, overlap=15):
    seqs = set(array[:,-1])
    master_list = []
    label_list = []
    
    for seq in seqs:
        label = info[seq,-1]
        sequence = array[array[:,-1] == seq]
        
        start = sequence[0][0]
        last = sequence[-1][0]
        stop = start+length
        while stop < last:
            #print stop, last
            sliced = slicing(sequence,start,stop)
            frsliced = slicing(facereader,start,stop)
            if sliced.shape[0] == 0:
                break
            sliced = sliced[:,1:-3]
            frsliced = frsliced[:,1:-1]
            sample = extract_features(frsliced, sliced, info)

            start += overlap
            stop += overlap

            master_list.append(sample)
            label_list.append([label,seq])

    return numpy.array(master_list,dtype=numpy.float32), numpy.array(label_list,dtype=numpy.int32)


def mean_imputation_sequences(allarray):

    allarraymeans = numpy.nanmean(allarray,0)
    
    locations = numpy.where(numpy.isnan(allarray))

    number = len(locations[0])
    for i in range(number):
        allarray[locations[0][i], locations[1][i]] = allarraymeans[locations[1][i]]

    return allarray
    

def build_confusion_matrix(predicted,true_values):
    confusion_matrix = numpy.zeros((4,4))
    true_values = list(true_values.astype(numpy.int32))

    label_sizes = [len(predicted)]
    for x in range(1,4):
        label_sizes.append(true_values.count(x))

    for pred,true in zip(predicted, true_values):
            confusion_matrix[0][0] += 1
            confusion_matrix[true][0] += 1
            confusion_matrix[0][pred] += 1
            confusion_matrix[true][pred] += 1.0/label_sizes[true]

    print confusion_matrix
    confusion_mean = (confusion_matrix[1][1]+confusion_matrix[2][2]+confusion_matrix[3][3])/float(len(set(true_values)))
    #confusion_mean = (confusion_matrix[1][1]+confusion_matrix[2][2])/2
    print "Confusion mean", confusion_mean
    overall_mean = sum(numpy.array(predicted)==numpy.array(true_values))/float(len(predicted))
    print "Overall mean", overall_mean
    #overall_mean = (confusion_matrix[1][1]*label_sizes[1]+confusion_matrix[2][2]*label_sizes[2]+confusion_matrix[3][3]*label_sizes[3])/len(predicted)
    #print "Over all", overall_mean
    #print "Participant mean", numpy.mean(scores)

    return confusion_mean, overall_mean

def divide_samples(samples, labels):
    training = []
    count = set()
    for i,row in enumerate(labels):
        current_seq = row[1]
        if row[0] not in count:
            count.add(row[0])
            j = i
            while current_seq == row[1]:
                current_seq = labels[j][1]
                training.append([j,labels[j][0]])
                j += 1
                if j >= labels.shape[0]-1:
                    break

    labels_col = fix_shape(labels[:,0])
    samples = numpy.hstack((samples,labels_col))
    indices = numpy.array(training)[:,0]
    training_array = samples[indices,:]
    print training_array.shape
    testing_array = numpy.delete(samples,indices,0)
    print testing_array.shape
    return training_array, testing_array


def which_features_seq(dataslice,eye=True,emotiv=True,facereader=True, onlyfacereader=False,onlyeye=False,onlyemotiv=False):
    if onlyfacereader:
        return dataslice[:,0:36]
    elif onlyeye:
        return dataslice[:,[36,42,48,54]]
    elif onlyemotiv:
        dataslice = dataslice[:,36:]
        dataslice = numpy.delete(dataslice,[0,6,12,18],1)
        return dataslice

    elif eye and emotiv and facereader:
        return dataslice
    elif not eye:
        features = numpy.delete(dataslice,[36,42,48,54],1)
        return features
    elif not emotiv:
        selection = range(60)
        for x in [36,42,48,54]:
            selection.remove(x)
        features = numpy.delete(dataslice,selection,1)
        print features.shape
        return features
    elif not facereader:
        return dataslice[:,36:]


def classify_sequences(individual=True):
    april = numpy.load(os.path.join(filepathApril, "sequentialDataApril.npy"))
    dec = numpy.load(os.path.join(filepathApril,"sequentialDataDecember.npy"))
    info = numpy.load("633samples60features.npy")

    april[:,-3] += 20 # -3: pnumber, -2 labels of success/fail, -1 sequence number
    april[:,1] = numpy.nan
    april[:,-1] += 313 

    alldata = numpy.vstack((dec,april))

    facereaderApril = numpy.load("allfacereaderApril.npy")
    facereaderDecember = numpy.load("allfacereaderDecember.npy")
    facereaderApril[:,-1] += 20
    allfacereader = numpy.vstack((facereaderDecember,facereaderApril))

    allfacereader[allfacereader == -1] = numpy.nan
    
    alldata = mean_imputation_sequences(alldata)
    allfacereader = mean_imputation_sequences(allfacereader)

    # best with 100, 0.001, SVM, 63.6_60.9

    gammas = numpy.logspace(-3,2,6)
    cs = numpy.logspace(-3,3,7)
    gammas = [0.001]
    cs = [10.0]

    #cs = [10,40,60,100,150,300]
    #gammas = [1,2,3,5,8,11,15,20]

    grid_scores = []
    for time_window in [90]:
        for overlap in [20]:
            for c in cs:
                for gamma in gammas:
            


                    pnumbers = set(alldata[:,-3]) 
                    allpreds = []
                    alltruth = []
                    scores = []
                    for pnumber in pnumbers:
                        print pnumber
                        participant = alldata[alldata[:,-3] == pnumber]
                        pfacereader = allfacereader[allfacereader[:,-1] == pnumber]
                        if individual:
                            predictions = []

                            #seqnumbers = set(participant[:,-1])
                            samplearray,labels = create_samples(info,participant,pfacereader,length=time_window,overlap=overlap)
                            if len(set(labels[:,0])) < 2:
                                print "Participant", pnumber, "has one class only"
                                continue
                            else:

                                scaler = preprocessing.MinMaxScaler((-1,1))
                                samplearray = scaler.fit_transform(samplearray.astype(numpy.float32))
                                samplearray=which_features_seq(samplearray)#,onlyemotiv=True)
                                training, test = divide_samples(samplearray, labels)
                                training_labels = training[:,-1]
                                test_labels = test[:,-1]
                                classifier = SVC(C=c,gamma=gamma,kernel="rbf",class_weight="auto")
                                #classifier = RandomForestClassifier(c,max_depth=gamma)#,class_weight="auto")
                                classifier.fit(training[:,0:-1],training_labels)
                                predictions.extend(classifier.predict(test[:,0:-1]))

                                """
                                for i,sample in enumerate(samplearray):
                                    classifier = SVC(C=10.0,gamma=0.001,kernel="rbf",class_weight="auto")
                                    training = numpy.delete(samplearray,[i],0)
                                    traininglabels = numpy.delete(labels,[i])
                                    
                                    classifier.fit(training,traininglabels)
                                    
                                    predictions.append(classifier.predict([sample])[0])
                                """

                            scores.append(build_confusion_matrix(predictions,test_labels))
                            allpreds.extend(predictions)
                            alltruth.extend(test_labels)
                            
                        else:
                            trainingData = alldata[alldata[:,-3] != pnumber]
                            trainingfacereader = allfacereader[allfacereader[:,-1] != pnumber]

                    print scores
                    print numpy.mean(scores)
                    conf_overall = build_confusion_matrix(allpreds,numpy.array(alltruth))
                    grid_scores.append((conf_overall[0],conf_overall[1],c,gamma,time_window,overlap))

    print sorted(grid_scores,reverse=True)


def which_features(dataslice,eye=True,emotiv=True,facereader=True, onlyfacereader=False,onlyeye=False,onlyemotiv=False):
    if onlyfacereader:
        return numpy.hstack((dataslice[:,0:36],dataslice[:,-6:]))
    elif onlyeye:
        return numpy.hstack((dataslice[:,[36,42,48,54]],dataslice[:,-6:]))
    elif onlyemotiv:
        dataslice = dataslice[:,36:]
        dataslice = numpy.delete(dataslice,[0,6,12,18],1)
        return dataslice

    elif eye and emotiv and facereader:
        return dataslice
    elif not eye:
        features = numpy.delete(dataslice,[36,42,48,54],1)
        return features
    elif not emotiv:
        selection = range(60)
        for x in [36,42,48,54]:
            selection.remove(x)
        features = numpy.delete(dataslice,selection,1)
        print features.shape
        return features
    elif not facereader:
        return dataslice[:,36:]

def classify_error_success(generate_files=False):
    if generate_files:
        alldata = numpy.load("allLogData2experiments.npy")

        facereaderApril = numpy.load("allfacereaderApril.npy")
        facereaderDecember = numpy.load("allfacereaderDecember.npy")

        alldata = stack_facereader(alldata, facereaderApril, facereaderDecember)

        #numpy.save("633samples60features",alldata)
    else:
        alldata = numpy.load("633samples60features.npy")

    scaler = preprocessing.MinMaxScaler((-1,1))
    bfarray = numpy.load("bfarray.npy")
    #bfarrayFeatures = preprocessing.scale(bfarray[:,0:-1].astype(numpy.float32),axis=0)
    bfarrayFeatures = scaler.fit_transform(bfarray[:,0:-1].astype(numpy.float32))
    bfarray = numpy.hstack((bfarrayFeatures,bfarray[:,-1:]))
    
    
    scaler = preprocessing.MinMaxScaler((-1,1))
    #features = preprocessing.scale(alldata[:,0:-6].astype(numpy.float32),axis=0)  
    features = scaler.fit_transform(alldata[:,0:-6].astype(numpy.float32))
    #alldata = numpy.hstack((features,alldata[:,-6:]))
    print alldata[0]
    #alldata = stack_big_five(alldata, bfarray)

    grid_search(alldata,chosen_label=-5)
    #leave_one_participant_out(alldata,1.0,0.01,chosen_label=-5) 56.2 SVM
    #leave_one_participant_out(alldata,0.001,0.0,chosen_label=-5) #56.4 LogisticRegression


def grid_search(alldata,chosen_label=-1):
    gammas = numpy.logspace(-9,3,13)
    cs = numpy.logspace(-6,4,11)
    gamma = 0.0
    scores = []
    for c in cs:
        #for gamma in gammas:
        scores.append((leave_one_participant_out(alldata,c,gamma,chosen_label),c,gamma))
    
    print sorted(scores,reverse=True)

def leave_one_participant_out(alldata, c=0.01, gamma=0.0, chosen_label=-1):

    confusion_matrix = numpy.zeros((4,4))

    alldata = which_features(alldata,onlyemotiv=True)
    print alldata.shape
    total = alldata.shape[0]

    pnumbers = alldata[:,-6]
    pnumbers = set(pnumbers)

    predicted = []
    true = []

    scores = []
    for pnumber in pnumbers:
        training = alldata[numpy.where(alldata[:,-6] != pnumber)]
        testing =  alldata[numpy.where(alldata[:,-6] == pnumber)]



        training_features = training[:,0:-6].astype(numpy.float32)
        training_labels = training[:,chosen_label].astype(numpy.float32).astype(numpy.int32)
        #training_labels = training[:,-5].astype(numpy.float32).astype(numpy.int32)

        testing_features = testing[:,0:-6].astype(numpy.float32)
        testing_labels = testing[:,chosen_label].astype(numpy.float32).astype(numpy.int32)
        #testing_labels = testing[:,-5].astype(numpy.float32).astype(numpy.int32)

        #print pnumber, testing.shape[0]

        #classifier = SVC(C=c,gamma=gamma,kernel="rbf",class_weight="auto")
        #classifier = LinearSVC(C=c,class_weight="auto")
        #classifier = SVC(C=1,class_weight="auto")
        classifier = LogisticRegression(C=c,class_weight="auto")
        #classifier = KNeighborsClassifier(7)
        #classifier = RandomForestClassifier(100,class_weight="auto")
        

        classifier.fit(training_features,training_labels)
        scores.append(classifier.score(testing_features,testing_labels))

        predicted.extend(classifier.predict(testing_features))
        true.extend(testing_labels)
        

    print "C, gamma",c,gamma
    print "Std of scores:",numpy.std(scores)

    label_sizes = [633]
    for x in range(1,4):
        label_sizes.append(true.count(x))
    truths = true
    for pred,true in zip(predicted, true):
            confusion_matrix[0][0] += 1
            confusion_matrix[true][0] += 1
            confusion_matrix[0][pred] += 1
            confusion_matrix[true][pred] += 1.0/label_sizes[true]

    print confusion_matrix
    confusion_mean = (confusion_matrix[1][1]+confusion_matrix[2][2]+confusion_matrix[3][3])/float(len(set(truths)))
    #confusion_mean = (confusion_matrix[1][1]+confusion_matrix[2][2])/2
    print "Confusion mean", confusion_mean
    overall_mean = (confusion_matrix[1][1]*label_sizes[1]+confusion_matrix[2][2]*label_sizes[2]+confusion_matrix[3][3]*label_sizes[3])/float(len(predicted))
    print "Over all", overall_mean
    print "Participant mean", numpy.mean(scores)

    return confusion_mean


if __name__ == "__main__":
    numpy.set_printoptions(suppress=True)
    
    if len(sys.argv) > 1:
        eval(sys.argv[1])
    else:
        #read_logs()
        #parse_questionnaire()
        #mean_imputation()
        #preprocess()
        #add_facereader(filepathApril)
        #read_logs_and_save_all(filepathDecember)
        classify_samples()
