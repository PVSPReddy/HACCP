﻿using System;
using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Extras
{
    /// <summary>
    ///     Parsers used for characteristics exposed by the TI SensorTag.
    ///     ATTRIBUTION
    ///     "TI SensorTag User Guide" by Texas Instruments, used under CC BY-SA 3.0 US / Desaturated from original
    ///     http://processors.wiki.ti.com/index.php/SensorTag_User_Guide
    /// </summary>
    internal class TI_BLESensorTagCharacteristicParsers
    {
        /// <summary>
        ///     Reads a 16-bit signed integer
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static long readInt16TI(DataReader reader)
        {
            uint ObjLSB = reader.ReadByte();
            int ObjMSB = (sbyte) reader.ReadByte();

            var result = (ObjMSB << 8) + ObjLSB;
            return result;
        }

        /// <summary>
        ///     Reads a 16-bit unsigned integer
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static long readUint16TI(DataReader reader)
        {
            uint ObjLSB = reader.ReadByte();
            int ObjMSB = reader.ReadByte();

            var result = (ObjMSB << 8) + ObjLSB;
            return result;
        }

        #region -------------------------- Temperature --------------------------

        public static string Parse_Temperature_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            if (value == 1)
            {
                return "1 -- Enabled";
            }
            if (value == 0)
            {
                return "0 -- Disabled";
            }
            return value + " -- Invalid";
        }

        public static string Parse_Temperature_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        public static string Parse_Temperature_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var objectTemp_int = readInt16TI(reader);
            var ambientTemp_int = readUint16TI(reader);

            // Do the math that the TI website tells us to
            var ambientTemp_dbl = calcTmpLocal(ambientTemp_int);
            var objectTemp_dbl = calcTmpTarget(objectTemp_int, ambientTemp_dbl);

            // Print out the string
            return "\nAmb: " + ambientTemp_dbl.ToString("F2") + "°C\nObj: " + objectTemp_dbl.ToString("F2") + "°C";
        }

        /* From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#IR_Temperature_Sensor
         * Conversion algorithm for die temperature */

        private static double calcTmpLocal(long rawT)
        {
            //-- calculate die temperature [°C] --
            return rawT/128.0; // Used in also in the calc. below
        }

        /* From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#IR_Temperature_Sensor
         * Conversion algorithm for target temperature */

        private static double calcTmpTarget(long rawT, double tAmb)
        {
            //-- calculate target temperature [°C] -
            double Vobj2 = rawT;
            Vobj2 *= 0.00000015625;

            var Tdie2 = tAmb + 273.15;
            const double S0 = 6.4E-14; // Calibration factor (C factor; might not be right for C#)
            const double a1 = 1.75E-3;
            const double a2 = -1.678E-5;
            const double b0 = -2.94E-5;
            const double b1 = -5.7E-7;
            const double b2 = 4.63E-9;
            const double c2 = 13.4;
            const double Tref = 298.15;
            var S = S0*(1 + a1*(Tdie2 - Tref) + a2*Math.Pow(Tdie2 - Tref, 2));
            var Vos = b0 + b1*(Tdie2 - Tref) + b2*Math.Pow(Tdie2 - Tref, 2);
            var fObj = Vobj2 - Vos + c2*Math.Pow(Vobj2 - Vos, 2);
            var tObj = Math.Pow(Math.Pow(Tdie2, 4) + fObj/S, 0.25);
            tObj = tObj - 273.15;

            return tObj;
        }

        #endregion // Temperature

        #region -------------------------- Accelerometer --------------------------

        public static string Parse_Accelerometer_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            if (value == 1)
            {
                return "1 -- Enabled";
            }
            if (value == 0)
            {
                return "0 -- Disabled";
            }
            return value + " -- Invalid";
        }

        public static string Parse_Accelerometer_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Accelerometer_2
        public static string Parse_Accelerometer_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var x = (sbyte) reader.ReadByte();
            var y = (sbyte) reader.ReadByte();
            var z = (sbyte) reader.ReadByte();


            var scaledX = x/64.0;
            var scaledY = y/64.0;
            var scaledZ = z/64.0;

            // Print out the string
            return "\ng = 9.81 m/s^2\nX: " + scaledX.ToString("F2") + "g\nY: " + scaledY.ToString("F2") + "g\nZ: " +
                   scaledZ.ToString("F2") + "g";
        }

        #endregion // Accelerometer

        #region -------------------------- Humidity --------------------------

        public static string Parse_Humidity_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            if (value == 1)
            {
                return "1 -- Enabled";
            }
            if (value == 0)
            {
                return "0 -- Disabled";
            }
            return value + " -- Invalid";
        }

        public static string Parse_Humidity_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        public static string Parse_Humidity_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var ambientTemp_int = readUint16TI(reader);
            var humidity_int = readUint16TI(reader);

            // Do the math that the TI website tells us to
            var ambientTemp_dbl = calcHumTmp(ambientTemp_int);
            var relHum_dbl = calcHumRel(humidity_int);

            // Print out the string
            return "\nAmbTemp: " + ambientTemp_dbl.ToString("F2") + "°C\nHum: " + relHum_dbl.ToString("F2") + "%RH";
        }

        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Humidity_Sensor_2
        private static double calcHumTmp(long rawT)
        {
            //-- calculate temperature [deg C] --
            return -46.85 + 175.72/65536*rawT;
        }

        private static double calcHumRel(long rawH)
        {
            rawH &= ~0x0003; // clear bits [1..0] (status bits)
            //-- calculate relative humidity [%RH] --
            return -6.0 + 125.0/65536*rawH; // RH= -6 + 125 * SRH/2^16
        }

        #endregion // Humidity

        #region -------------------------- Magnetometer --------------------------

        public static string Parse_Magnetometer_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            if (value == 1)
            {
                return "1 -- Enabled";
            }
            if (value == 0)
            {
                return "0 -- Disabled";
            }
            return value + " -- Invalid";
        }

        public static string Parse_Magnetometer_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        public static string Parse_Magnetometer_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var X_long = readInt16TI(reader);
            var Y_long = readInt16TI(reader);
            var Z_long = readInt16TI(reader);

            // Do the math that the TI website tells us to
            /* NOTE: There is a comment on the TI website that says 
             * "// Multiply x and y with -1 so that the values correspond with our pretty pictures in the app."
             * You might want to do that if the values look off. */
            var X_dbl = calcMagn(X_long);
            var Y_dbl = calcMagn(Y_long);
            var Z_dbl = calcMagn(Z_long);

            // Print out the string
            return "\nX: " + X_dbl.ToString("F2") + "uT\nY: " + Y_dbl.ToString("F2") + "uT\nZ: " + Z_dbl.ToString("F2") +
                   "uT";
        }

        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Magnetometer
        /* Converting to microTeslas */

        private static double calcMagn(long rawX)
        {
            //-- calculate magnetic-field strength, unit uT, range -1000, +1000
            return (double) rawX/(65536/2000);
        }

        #endregion // Magnetometer

        #region -------------------------- Barometer --------------------------

        // The barometer needs to read in calibration data before outputting values. 
        // Having a static array here is a little hacky, but works for now.

        public static ushort[] BarCalibUnsigned = {0, 0, 0, 0};
        public static short[] BarCalibSigned = {0, 0, 0, 0};

        public static string Parse_Barometer_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            if (value == 1)
            {
                return "1 -- Enabled";
            }
            if (value == 0)
            {
                return "0 -- Disabled";
            }
            if (value == 2)
            {
                return "2 -- \nReading Calibration Data";
            }
            return value + " -- Invalid";
        }

        public static string Parse_Barometer_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        public static string Parse_Barometer_Calibration(IBuffer buffer)
        {
            if (buffer.Length < 16)
            {
                return "Calibration not requested";
            }

            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            for (var i = 0; i < 4; i++)
            {
                BarCalibUnsigned[i] = (ushort) readUint16TI(reader);
            }
            for (var i = 0; i < 4; i++)
            {
                BarCalibSigned[i] = (short) readInt16TI(reader);
            }

            // Build the calibration string.
            var result = "";
            for (var i = 0; i < 4; i++)
            {
                result += "\n" + BarCalibUnsigned[i];
            }
            for (var i = 0; i < 4; i++)
            {
                result += "\n" + BarCalibSigned[i];
            }

            // Print out the string
            return result;
        }

        public static string Parse_Barometer_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var ambientTemp_int = readInt16TI(reader);
            var pressure_int = readUint16TI(reader);

            // Do the math that the TI website tells us to
            var ambientTemp_dbl = calcBarTmp(ambientTemp_int);
            var pressure_dbl = calBarPress(ambientTemp_int, pressure_int);

            // Print out the string
            return "\nAmbTemp: " + ambientTemp_dbl.ToString("F2") + "°C\nPressure: " + pressure_dbl.ToString("F2") +
                   " hP";
        }

        // Converts temperature reading from barometer sensor to actual temperature.
        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Barometric_Pressure_Sensor_2
        private static double calcBarTmp(long rawTemp)
        {
            return (double) (((BarCalibUnsigned[0]*rawTemp*100) >> 24) + (((long) BarCalibUnsigned[1]*100) >> 10))/100;
        }

        // Converts pressure from barometer sensor to actual pressure
        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Barometric_Pressure_Sensor_2
        private static double calBarPress(long rawTemp, long rawPres)
        {
            var sensitivity = BarCalibUnsigned[2] + ((BarCalibUnsigned[3]*rawTemp) >> 17) +
                              ((BarCalibSigned[0]*rawTemp*rawTemp) >> 34);
            var offset = ((long) BarCalibSigned[1] << 14) + ((BarCalibSigned[2]*rawTemp) >> 3) +
                         ((BarCalibSigned[3]*rawTemp*rawTemp) >> 19);
            return (double) ((sensitivity*rawPres + offset) >> 14)/100;
        }

        #endregion // Barometer

        #region -------------------------- Gyroscope --------------------------

        public static string Parse_Gyroscope_Configuration(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            switch (value)
            {
                case 0:
                    return "0 -- Disabled";

                case 1:
                    return "1 -- X-axis only";

                case 2:
                    return "2 -- Y-axis only";

                case 3:
                    return "3 -- X and Y-axes only";

                case 4:
                    return "4 -- Z axis only";

                case 5:
                    return "5 -- X and Z-axes only";

                case 6:
                    return "6 -- Y and Z-axes only";

                case 7:
                    return "7 -- X, Y and Z-axes";

                default:
                    return value + " -- Invalid";
            }
        }

        public static string Parse_Gyroscope_Period(IBuffer buffer)
        {
            var value = DataReader.FromBuffer(buffer).ReadByte();
            return value + "0 ms";
        }

        public static string Parse_Gyroscope_Data(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);

            // Parse in correct endianness
            var x_int = readInt16TI(reader);
            var y_int = readInt16TI(reader);
            var z_int = readInt16TI(reader);

            // Do the math that the TI website tells us to
            var x = (float) x_int*500/65536;
            var y = (float) y_int*500/65536*-1;
            var z = (float) z_int*500/65536;

            // Print out the string
            return "\nX: " + x.ToString("F2") + " dps\nY: " + y.ToString("F2") + " dps\nZ: " + z.ToString("F2") + " dps";
        }

        // From http://processors.wiki.ti.com/index.php/SensorTag_User_Guide#Gyroscope

        #endregion // Gyroscope
    }
}