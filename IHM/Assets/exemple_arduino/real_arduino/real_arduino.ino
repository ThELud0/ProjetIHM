int axeX = A0;      // signal de l'axe X sur entrée A0
int axeY = A1;      // signal de l'axe Y sur entrée A1
int jumpBtPin = 7;  // Bouton-poussoir en broche 7

char jumpMessageType = 'J';

bool jumpReleased;
unsigned long justJumped, justUpdatedPrint;
const unsigned long jumpPressDelay = 200, printDelay = 500;


void setup() {
  pinMode(axeX, INPUT);       // définition de A0 comme une entrée
  pinMode(axeY, INPUT);       // définition de A1 comme une entrée
  pinMode(jumpBtPin, INPUT);  // définition de 2 comme une entrée
  digitalWrite(jumpBtPin, HIGH); // Activation de la résistance de Pull-Up interne de la carte Uno 

  Serial.begin(9600);
  justJumped = millis();
  justUpdatedPrint = millis();
  jumpReleased = true;
}


void loop() {
  float X, Y;
  X = analogRead(axeX) * (5.0 / 1023.0);
  Y = analogRead(axeY) * (5.0 / 1023.0);

  int buttonRead;

  buttonRead = digitalRead(jumpBtPin);



  if (millis() > justUpdatedPrint + printDelay) {
    justUpdatedPrint = millis();
    Serial.print("Axe X:");
    Serial.print(X, 4);
    Serial.print("V, ");
    Serial.print("Axe Y:");
    Serial.print(Y, 4);
    Serial.println("V");
  }
  if (buttonRead == 1)
    jumpReleased = true;
  if ((buttonRead != 1) && (millis() > justJumped + jumpPressDelay) && jumpReleased){
    justJumped = millis();
    jumpReleased = false;
    Serial.println("Bouton-poussoir actif");
    Serial.println(jumpMessageType);
  }
  delay(10);
}

void sendJumpMessage(){
  sendMessage(jumpMessageType, 0, nullptr);
}

// Function to send messages with a fixed header
void sendMessage(char type, uint8_t length, uint8_t* data) {
  Serial.write(type);  // Write the message type
  Serial.write(length);  // Write the length of the payload
  if (data != nullptr && length > 0) {
    Serial.write(data, length);  // Write the payload if it exists
  }
}
