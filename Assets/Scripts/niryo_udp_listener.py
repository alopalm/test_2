import socket
import json
from pyniryo import NiryoRobot, PoseObject

ROBOT_IP = "192.168.1.104"   # IP real del Ned2
UDP_IP = "0.0.0.0"           # escuchar en todas las interfaces
UDP_PORT = 5055

def clamp(value, vmin, vmax):
    return max(vmin, min(value, vmax))

print("Conectando con el robot...")
robot = NiryoRobot(ROBOT_IP)

try:
    try:
        robot.calibrate_auto()
    except Exception as e:
        print("Aviso al calibrar:", e)

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((UDP_IP, UDP_PORT))
    print(f"Escuchando UDP en {UDP_IP}:{UDP_PORT}")

    while True:
        data, addr = sock.recvfrom(4096)
        text = data.decode("utf-8")
        print("Recibido de Unity:", text)

        msg = json.loads(text)

        x = clamp(float(msg["x"]), 0.15, 0.35)
        y = clamp(float(msg["y"]), -0.20, 0.20)
        z = clamp(float(msg["z"]), 0.10, 0.35)

        roll = float(msg.get("roll", -3.14))
        pitch = float(msg.get("pitch", 0.0))
        yaw = float(msg.get("yaw", 0.0))

        print(f"Moviendo robot a: {x}, {y}, {z}, {roll}, {pitch}, {yaw}")
        robot.move(PoseObject(x, y, z, roll, pitch, yaw))

except KeyboardInterrupt:
    print("Parado por usuario.")
finally:
    try:
        robot.close_connection()
    except:
        pass
