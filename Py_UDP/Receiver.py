import socket

# Define the UDP server address and port (same as the sender)
UDP_IP = "127.0.0.1"  # Localhost, should match the sender's IP address
UDP_PORT = 5005       # Port to receive data on (should match the sender's port)

# Initialize the UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((UDP_IP, UDP_PORT))

# Function to log the received data
def log_data(message):
    with open("received_data.log", "a") as log_file:
        log_file.write(message + "\n")
    print(f"Logged: {message}")

# Listening for incoming data and logging it
print(f"Receiver is listening on {UDP_IP}:{UDP_PORT}...")
try:
    while True:
        # Receive data
        data, addr = sock.recvfrom(1024)  # Buffer size is 1024 bytes
        received_message = data.decode("utf-8")

        # Log the received message
        log_data(received_message)

except KeyboardInterrupt:
    print("\nReceiver program interrupted. Closing...")
finally:
    sock.close()
