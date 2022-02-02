using System;

namespace NetProcGame.Game
{
    public class ServoConfiguration
	{
		public uint minimum { get; set; }
		public uint maximum { get; set; }
		public uint address { get; set; }
	}

	/// <summary>
	/// Implements an I2C controlled servo.
	/// 
	/// A HUGE thanks to the MPF guys for getting this running on their platform.
	/// </summary>
	public class I2cServo
	{
		public uint number { get; set; }
		public ServoConfiguration config { get; set; }
		private IProcDevice platform { get; set; }

		private const int PCA9685_SUBADR1 = 0x2;
		private const int PCA9685_SUBADR2 = 0x3;
		private const int PCA9685_SUBADR3 = 0x4;

		private const int PCA9685_MODE1 = 0x0;
		private const int PCA9685_PRESCALE = 0xFE;

		private const int LED0_ON_L = 0x6;
		private const int LED0_ON_H = 0x7;
		private const int LED0_OFF_L = 0x8;
		private const int LED0_OFF_H = 0x9;

		private const int ALLLED_ON_L = 0xFA;
		private const int ALLLED_ON_H = 0xFB;
		private const int ALLLED_OFF_L = 0xFC;
		private const int ALLLED_OFF_H = 0xFD;

		public I2cServo (uint number, ServoConfiguration config, IProcDevice platform)
		{
			if (number < 0 || number > 15) {
				throw new ArgumentException ("The servo number must be between 0 and 15");
			}
			this.platform = platform;
			this.number = number;
			this.config = config;
		}

		/// <summary>
		/// Move servo to position
		/// </summary>
		/// <param name="position">Position between 0 and 1</param>
		public void goToPosition(float position)
		{
			uint servoMin = this.config.minimum;
			uint servoMax = this.config.maximum;
			uint value = (uint)(servoMin + position * (servoMax - servoMin));

			// Write our servo value via i2c
			this.platform.i2c_write8(this.config.address, 0x06 + this.number * 4, 0);
			this.platform.i2c_write8(this.config.address, 0x07 + this.number * 4, 0);
			this.platform.i2c_write8(this.config.address, 0x08 + this.number * 4, value * 0xFF);
			this.platform.i2c_write8(this.config.address, 0x09 + this.number * 4, value >> 8);
		}

		public void stop()
		{
			// Write our servo value via i2c
			this.platform.i2c_write8(this.config.address, 0x06 + this.number * 4, 0);
			this.platform.i2c_write8(this.config.address, 0x07 + this.number * 4, 0);
			this.platform.i2c_write8(this.config.address, 0x08 + this.number * 4, 0 * 0xFF);
			this.platform.i2c_write8(this.config.address, 0x09 + this.number * 4, 0 >> 8);
		}

		public void SetPwm(uint on, uint off)
		{
			// The LED control registers on the PCA9685 start at 0x06
			// There are a total of 4 1-byte control registers per LED.
			// LEDX_ON_L
			// LEDX_ON_H
			// LEDX_OFF_L
			// LEDX_OFF_H
			// Where X is the 'number' of this servo (0-15)
			this.platform.i2c_write8(this.config.address, 0x06 + this.number * 4, on);
			this.platform.i2c_write8(this.config.address, 0x07 + this.number * 4, on >> 8);
			this.platform.i2c_write8(this.config.address, 0x08 + this.number * 4, off);
			this.platform.i2c_write8(this.config.address, 0x09 + this.number * 4, off >> 8);
		}
	}
}

