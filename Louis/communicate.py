import numpy

import datetime
import time
import os
import sys

logpath = "Log\\";
filepath = "Louis-JVI-log.txt"
facepath = "Louis-JVI-FaceReaderLog.txt"
decisionpath = "Louis-JVI-decision.txt"

def classification(segment):
    if int((segment[-1][-1]%1) * 10) == 1: #Bogus line to test things
        return "1"
    else:
        return "0"

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
        for i in range(segment.shape[1]):
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

def text_to_array(log, segment, face_segment):
    lines = [line.strip().split(" ") for line in segment]
    for line in lines:
        line[-1] = time_to_seconds(line[-1])
    
    segment = numpy.array(lines)
    
    segment[segment == "FIT_FAILED"] = numpy.nan
    segment[segment == "FIND_FAILED"] = numpy.nan
    segment[segment == "-1"] = numpy.nan
    segment[0][1] = numpy.nan
    
    segment = segment.astype(numpy.float64)

    lines = [for line in face_segment]
    for line in lines:
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

    segment = numpy.hstack((to_stack,segment))

    segment = data_imputation(log, segment)

    return segment

def process_facereader(segment):
    segment = [line.strip().split("    ") for line in segment]
    final_segment = []
    for line in segment:
        if len(line) > 10:
            line = line[1:11]
            final_line = [value.strip().split(" ")[-1] for value in line]
            final_segment.append(final_line)

    return final_segment

def write_back(decision,time):
    with open(os.path.join(logpath,decisionpath),"a") as opened_file:
        opened_file.write(decision+","+str(time)+"\n")

def main():
    log = []
    while filepath not in os.listdir(logpath):
        time.sleep(10)
    with open(os.path.join(logpath,filepath),"r") as opened_file:
        with open(os.path.join(logpath,facepath),"r") as face_file:
            while True: 
                segment = opened_file.readlines()
                face_segment = face_file.readlines()
                if len(face_segment) > 1:
                    face_segment = process_facereader(face_segment)
                if len(segment) > 1:
                    print segment[0], segment[-1]
                    segment = text_to_array(log,segment,face_segment)
                    log.append(segment)
                    print segment.shape, segment[0][-1], segment[-1][-1], segment.dtype
                    decision = classification(segment)
                    write_back(decision,segment[-1][0])
                elif len(log) > 1:
                    numpy.save(os.path.join(logpath,"currentparticipant"),numpy.vstack(log))
                time.sleep(10)


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