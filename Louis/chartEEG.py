import matplotlib.pyplot as plt

length = 1024 # Number of samples
order = 5 # Order of the filter
sample_rate = 128 # Hz, Emotiv specifications
cutoff_freq = 0.16 # Hz, recommendation by Emotiv
bands = [0.5,4,7,12,30] # Limits of bands of interests (in Hz too)

numpy.set_printoptions(suppress=True)

# Creating a filter and returning its coefficients	
nyquist = 0.5*sample_rate
cutoff = cutoff_freq / nyquist
b_coef, a_coef = signal.butter(order, cutoff, btype="high", analog = False)

# Creating a window function
hannwindow = numpy.hanning(length)


def main():
	filepath = "Log\\EEG.csv"
	plotted_ratio = [25,25,25,25]

	plt.ion()
	labels = 'Alpha', 'Beta', 'Theta', 'Delta'
	sizes = plotted_ratio
	colors = ['yellowgreen', 'gold', 'lightskyblue', 'lightcoral']

	plt.pie(sizes, labels=labels, colors=colors,
	        autopct='%1.1f%%', shadow=True, startangle=90)
	# Set aspect ratio to be equal so that pie is drawn as a circle.
	plt.axis('equal')

	plt.show()

	while True:

		for seq in channels:
			seq = signal.lfilter(b_coef, a_coef, seq)
			windowed_seq = seq*hannwindow
			power, power_ratios = pyeeg.bin_power(windowed_seq, bands, sample_rate)
			plotted_ratio 


			# Need to check something first

main()