using System;

namespace NetProcGame
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
			uint value = (uint)(servoMin + position * (servoMax - servoMax));

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
	}
}

