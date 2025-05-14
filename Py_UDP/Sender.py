import socket
import time
import math

# Define the UDP target address and port
UDP_IP = "127.0.0.1"  # Localhost, change to the receiver's IP if needed
UDP_PORT = 5005  # Port to send data to

# Define the device name (Tx or Rx)
device_names = ["Tx", "Rx"]

# Define the ranges for each parameter
# RSSI (dBm)
min_rssi = -100
max_rssi = -60

# SINR (dB)
min_sinr = 0
max_sinr = 40

# Data Rate (Mbps)
min_data_rate = 0
max_data_rate = 5

# Gain (dB)
min_gain = 0
max_gain = 30

# Azimuth and Elevation Angles (degrees)
min_azimuth = -90
max_azimuth = 90
min_elevation = -45
max_elevation = 45

# Define the frequency of sending data (in seconds)
frequency = 0.25  # 1/4 second interval between each data packet

# Initialize the UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


# Function to simulate the dynamic values for each parameter
def get_dynamic_values():
    timestamp = time.time()

    # Simulate the RSSI, SINR, Data Rate, Gain, and Angles
    rssi = min_rssi + (max_rssi - min_rssi) * (math.sin(timestamp) + 1) / 2
    sinr = min_sinr + (max_sinr - min_sinr) * (math.cos(timestamp) + 1) / 2
    data_rate = min_data_rate + (max_data_rate - min_data_rate) * (math.sin(timestamp) + 1) / 2
    gain = min_gain + (max_gain - min_gain) * (math.cos(timestamp) + 1) / 2
    azimuth = min_azimuth + (max_azimuth - min_azimuth) * (math.sin(timestamp) + 1) / 2
    elevation = min_elevation + (max_elevation - min_elevation) * (math.cos(timestamp) + 1) / 2

    return rssi, sinr, data_rate, gain, azimuth, elevation


# Function to send data over UDP
def send_data(device_name, rssi, sinr, data_rate, gain, azimuth, elevation):
    message = (
        f"Device: {device_name}, "
        f"RSSI: {rssi:.2f} dBm, "
        f"SINR: {sinr:.2f} dB, "
        f"Data Rate: {data_rate:.2f} Mbps, "
        f"Gain: {gain:.2f} dB, "
        f"Azimuth: {azimuth:.2f} degrees, "
        f"Elevation: {elevation:.2f} degrees"
    )
    sock.sendto(message.encode(), (UDP_IP, UDP_PORT))
    print(f"Sent: {message}")


# Main loop to continuously send data
try:
    while True:
        for device_name in device_names:
            # Get dynamic values for each parameter
            rssi, sinr, data_rate, gain, azimuth, elevation = get_dynamic_values()

            # Send the data over UDP
            send_data(device_name, rssi, sinr, data_rate, gain, azimuth, elevation)

            # Wait for the next transmission cycle
            time.sleep(frequency)

except KeyboardInterrupt:
    print("\nProgram interrupted. Closing...")
finally:
    sock.close()
