import numpy

import datetime
import time
import os
import sys

logpath = "Log\\";
filepath = "Louis-JVI-log.txt"
decisionpath = "Louis-JVI-decision.txt"

def classification(segment):
	if int((segment[-1][-1]%1) * 10) == 1: #Bogus line to test things
		return "Help"
	else:
		return "Nothing"

def seconds_to_time(timeseconds):
    return str(datetime.timedelta(seconds=timeseconds))
 
def time_to_seconds(timestring):
    remainder = float(timestring.split('.')[1])/1000.0
    x = time.strptime(timestring.split('.')[0],'%H:%M:%S')
    return datetime.timedelta(hours=x.tm_hour,minutes=x.tm_min,seconds=x.tm_sec).total_seconds()+remainder

def text_to_array(segment):
	lines = [line.strip().split(" ") for line in segment]
	for line in lines:
		line[-1] = time_to_seconds(line[-1])
	return numpy.array(lines,dtype=numpy.float64)

def write_back(decision,time):
	with open(os.path.join(logpath,decisionpath),"a") as opened_file:
		opened_file.write(decision+","+str(time)+"\n")

def main():
	while filepath not in os.listdir(logpath):
		time.sleep(10)
	with open(os.path.join(logpath,filepath),"r") as opened_file:
		while True:	
			segment = opened_file.readlines()
			if len(segment) > 1:
				print segment[0], segment[-1]
				segment = text_to_array(segment)
				print segment.shape, segment[0][-1], segment[-1][-1], segment.dtype
				decision = classification(segment)
				write_back(decision,segment[-1][0])
			time.sleep(10)

main()