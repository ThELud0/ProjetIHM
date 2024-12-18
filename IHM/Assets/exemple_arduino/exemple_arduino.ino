// The flag signals to the rest of the program an interrupt occured
static bool button_flag = false;

int sensorValue = 0;  // value read from the pot
int outputValue = 0;
const int analogInPin = A0; 

// Interrupt handler, sets the flag for later processing
void buttonPress() {
  button_flag = true;
}

void setup() {
  int buttonPin = 2;
  
  pinMode(13, OUTPUT);
  digitalWrite(13,HIGH);
  // Internal pullup, no external resistor necessary
  pinMode(buttonPin,INPUT_PULLUP);
  // 115200 is a common baudrate : fast without being overwhelming
  Serial.begin(115200);

  // As the button is in pullup, detect a connection to ground
  attachInterrupt(digitalPinToInterrupt(buttonPin),buttonPress,FALLING);

  // Wait for a serial connection
  while (!Serial.availableForWrite());
  // In case the Unity project isn't synced with the boolean.
  sendMessage('D', 0, nullptr);

}

// Processes button input
void loop() {
  // Slows reaction down a bit
  // but prevents _most_ button press misdetections
  //delay(300);
  analogControl();
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\n');
  if (message == "LED ON") {
    digitalWrite(13,LOW);
  } else if (message == "LED OFF") {
    digitalWrite(13,HIGH);
  }
}

void analogControl(){
  // read the analog in value:
  sensorValue = analogRead(analogInPin);
  // map it to the range of the analog out:
  outputValue = map(sensorValue, 0, 1023, 0, 255);

  uint8_t payload = outputValue;  // Payload is a single byte
  sendMessage('S', 1, &payload);  // Type = 'S', Length = 1

  delay(20);
}

// Function to send messages with a fixed header
void sendMessage(char type, uint8_t length, uint8_t* data) {
  Serial.write(type);  // Write the message type
  Serial.write(length);  // Write the length of the payload
  if (data != nullptr && length > 0) {
    Serial.write(data, length);  // Write the payload if it exists
  }
}
