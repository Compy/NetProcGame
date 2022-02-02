# RGB leds

Basic Example to test `PD-RGB` led connections from P3-ROC

Use the `machine.json` to set the name / addresses for the LEDs. This example is set up so the first board is a `PD-LED` connection from the `P3-ROC` jumper 12.

When run, it will cycle colours on the first 5 LEDS on board address 0. 

```
LED1 = A0-R0-B1-G2
LED2 = A0-R3-B4-G5
LED3 = A0-R6-B7-G8
LED4 = A0-R9-B10-G11
LED5 = A0-R12-B13-G14
```