using System;
using System.IO.Ports;

namespace NetProcGame.Ports
{
	public enum GroupMatchType : byte {
		ANY = 0x01,
		EXACT = 0x02,
		ALL = 0x03
	}
	/// <summary>
	/// A driver interface for WS2811/WS2812 LED chains driven from a Teensy OctoWS2811 adapter
	/// </summary>
	public class WSLEDDriver
	{
		/// <summary>
		/// Serial port instance
		/// </summary>
		private SerialPort _driver;

		/// <summary>
		/// The DateTime this instance was created
		/// </summary>
		private DateTime _epoch;

		/// <summary>
		/// The number of LEDs in the chain
		/// </summary>
		private int _ledCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="NetProcGame.WSLEDDriver"/> class.
		/// </summary>
		/// <param name="port">The serial port the device is connected to</param>
		public WSLEDDriver (string port, int ledCount = 50, int baudrate = 115200)
		{
			_epoch = DateTime.Now;
			_driver = new SerialPort (port, baudrate);
			_driver.ReadTimeout = 10000;
			_ledCount = ledCount;
			if (_driver == null) {
				throw new Exception ("LED driver serial port could not be initialized.");
			}
			_driver.Open ();
		}

		/// <summary>
		/// Get milliseconds since this instance was created
		/// </summary>
		public long Millis()
		{
			return (long)(DateTime.Now - _epoch).TotalMilliseconds;
		}

		/// <summary>
		/// Assigns the lamp to the given group
		/// </summary>
		/// <param name="lampIndex">Lamp index</param>
		/// <param name="groupBits">Group number</param>
		public void AssignLamp(byte lampIndex, byte groupBits)
		{
			byte byte1 = 15;
			byte1 <<= 4;
			byte1 = (byte)(byte1 | (lampIndex & Convert.ToUInt32("111100000000", 2)));
			byte byte2 = (byte)(lampIndex & Convert.ToUInt32 ("000011111111", 2));

			byte[] buff = new byte[7] { byte1, byte2, groupBits, 0, 0, 0, 0 };

			_driver.Write (buff, 0, buff.Length);
		}

		/// <summary>
		/// Fades the given group to the given color
		/// </summary>
		/// <param name="groupBits">Group number</param>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="time">How many time steps to fade over</param>
		/// <param name="matchType">Match type.</param>
		public void GroupFadeToColor(byte groupBits, byte r, byte g, byte b, byte time, GroupMatchType matchType = GroupMatchType.ANY)
		{
			byte byte1 = 9;
			byte1 <<= 4;
			byte1 = (byte)(byte1 | (byte)matchType);
			byte byte2 = groupBits;

			byte[] buff = new byte[7] { byte1, byte2, r, g, b, time, 0 };
			_driver.Write (buff, 0, buff.Length);
		}

		/// <summary>
		/// Schedules a given group
		/// </summary>
		/// <param name="groupBits">Group number</param>
		/// <param name="schedule">32-bit schedule</param>
		/// <param name="now">If set to <c>true</c> schedule now.</param>
		/// <param name="matchType">Match type.</param>
		public void GroupSchedule(byte groupBits, UInt32 schedule, bool now, GroupMatchType matchType = GroupMatchType.ANY)
		{
			byte byte1 = 11;
			if (now) {
				byte1 = 10;
			}

			byte1 <<= 4;
			byte1 = (byte)(byte1 | (byte)matchType);

			byte byte2 = groupBits;
			byte byte3 = (byte)((schedule & Convert.ToUInt32 ("11111111000000000000000000000000", 2)) >> 24);
			byte byte4 = (byte)((schedule & Convert.ToUInt32 ("00000000111111110000000000000000", 2)) >> 16);
			byte byte5 = (byte)((schedule & Convert.ToUInt32 ("00000000000000001111111100000000", 2)) >> 8);
			byte byte6 = (byte)(schedule & Convert.ToUInt32 ("00000000000000000000000011111111", 2));
			byte byte7 = 0x00;

			byte[] buff = new byte[7] { byte1, byte2, byte3, byte4, byte5, byte6, byte7 };
			_driver.Write (buff, 0, buff.Length);
		}

		/// <summary>
		/// Schedules the given lamp with the given schedule
		/// </summary>
		/// <param name="lampIndex">Lamp index to schedule</param>
		/// <param name="schedule">32 bit on/off schedule</param>
		/// <param name="extraLamps">Extra lamps after the given lamp to turn off</param>
		public void ScheduleLamp(byte lampIndex, UInt32 schedule, byte extraLamps = 0x00)
		{
			byte byte1 = 2;
			byte1 <<= 4;
			byte1 = (byte)(byte1 | (byte)(lampIndex & Convert.ToUInt32 ("111100000000", 2)));
			byte byte2 = (byte)(lampIndex & Convert.ToUInt32 ("000011111111", 2));
			byte byte3 = (byte)((schedule & Convert.ToUInt32 ("11111111000000000000000000000000", 2)) >> 24);
			byte byte4 = (byte)((schedule & Convert.ToUInt32 ("00000000111111110000000000000000", 2)) >> 16);
			byte byte5 = (byte)((schedule & Convert.ToUInt32 ("00000000000000001111111100000000", 2)) >> 8);
			byte byte6 = (byte)((schedule & Convert.ToUInt32 ("00000000000000000000000011111111", 2)));
			byte byte7 = extraLamps;

			byte[] buff = new byte[7] { byte1, byte2, byte3, byte4, byte5, byte6, byte7 };
			_driver.Write (buff, 0, buff.Length);
		}

		/// <summary>
		/// Schedules all LEDs in the chain to the given schedule
		/// </summary>
		/// <param name="schedule">32 bit on/off schedule</param>
		public void ScheduleAll(UInt32 schedule)
		{
			for (byte i = 0; i < _ledCount; i++) {
				ScheduleLamp (i, schedule, 0);
			}
		}

		/// <summary>
		/// Fades all LEDs to the given color over the given time length
		/// </summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="time">Time steps to fade over</param>
		public void FadeAllToColor(byte r, byte g, byte b, byte time = 1)
		{
			for (byte i = 0; i < _ledCount; i++) {
				byte byte1 = 1;
				byte1 <<= 4;
				byte1 = (byte)(byte1 | (i & Convert.ToUInt32("111100000000",2)));
				byte byte2 = (byte)(i & Convert.ToUInt32("000011111111",2));

				byte[] buff = new byte[7] { byte1, byte2, r, g, b, time, 0 };
				_driver.Write (buff, 0, buff.Length);
			}
			_driver.BaseStream.Flush ();
		}

		public void FadeLedToColor(byte led, byte r, byte g, byte b, byte time = 1) 
		{
			byte byte1 = 1;
			byte1 <<= 4;
			byte1 = (byte)(byte1 | (led & Convert.ToUInt32("111100000000",2)));
			byte byte2 = (byte)(led & Convert.ToUInt32("000011111111",2));

			byte[] buff = new byte[7] { byte1, byte2, r, g, b, time, 0 };
			_driver.Write (buff, 0, buff.Length);
		}

		/// <summary>
		/// Closes the connection to the LED driver
		/// </summary>
		public void Close()
		{
			_driver.Close ();
		}
	}
}

