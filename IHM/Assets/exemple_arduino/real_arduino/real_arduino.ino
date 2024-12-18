int axeX = A0;      // signal de l'axe X sur entrée A0
int axeY = A1;      // signal de l'axe Y sur entrée A1
int jumpBtPin = 7;  // Bouton-poussoir en broche 7

char jumpMessageType = 'J';
char movementMessageXType = 'X';
char movementMessageYType = 'Y';

bool jumpReleased;
unsigned long justJumped, justUpdatedPrint;
const unsigned long jumpPressDelay = 200, printDelay = 500;
int outputValueX = 0;
int outputValueY = 0;

bool activateSendMessages = false;
bool activateReceiveMessages = false;

void setup() {
  pinMode(axeX, INPUT);           // définition de A0 comme une entrée
  pinMode(axeY, INPUT);           // définition de A1 comme une entrée
  pinMode(jumpBtPin, INPUT);      // définition de 2 comme une entrée
  digitalWrite(jumpBtPin, HIGH);  // Activation de la résistance de Pull-Up interne de la carte Uno

  Serial.begin(9600);
  justJumped = millis();
  justUpdatedPrint = millis();
  jumpReleased = true;
}


void loop() {

  int buttonRead;

  if (millis() > justUpdatedPrint + printDelay) {

    justUpdatedPrint = millis();
    Serial.println("Mouvement mis à jour");
    sendMovementMessage();
  }

  buttonRead = digitalRead(jumpBtPin);
  if (buttonRead == 1)
    jumpReleased = true;
  if ((buttonRead != 1) && (millis() > justJumped + jumpPressDelay) && jumpReleased) {
    justJumped = millis();
    jumpReleased = false;
    Serial.println("Bouton-poussoir actif");
    sendJumpMessage();
  }
  delay(10);
}

void sendMovementMessage() {
  int X, Y;
  X = analogRead(axeX);
  Y = analogRead(axeY);
  Serial.print("Axe X:");
  Serial.print(X);
  Serial.print(", ");
  Serial.print("Axe Y:");
  Serial.print(Y);
  Serial.println("");
  outputValueX = map(X, 0, 1023, 0, 255) - 126;
  outputValueY = map(Y, 0, 1023, 0, 255) - 124;
  Serial.print("Axe mapped X:");
  Serial.print(outputValueX);
  Serial.print(", ");
  Serial.print("Axe mapped Y:");
  Serial.print(outputValueY);
  Serial.println("");
  uint8_t payloadX = outputValueX;  // Payload is a single byte
  uint8_t payloadY = outputValueY;
  sendMessage(movementMessageXType, 1, &payloadX);
  sendMessage(movementMessageYType, 1, &payloadY);
}

void sendJumpMessage() {
  sendMessage(jumpMessageType, 0, nullptr);
}


// Function to send messages with a fixed header
void sendMessage(char type, uint8_t length, uint8_t* data) {

  if (activateSendMessages) {
    Serial.write(type);    // Write the message type
    Serial.write(length);  // Write the length of the payload
    if (data != nullptr && length > 0) {
      Serial.write(data, length);  // Write the payload if it exists
    }
  }

}
