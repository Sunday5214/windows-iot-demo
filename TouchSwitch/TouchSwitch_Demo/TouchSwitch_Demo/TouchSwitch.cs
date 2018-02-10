﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace TouchSwitch_Demo
{
    /// <summary>
    /// Switch State
    /// </summary>
    enum SwitchState
    {
        OFF,
        ON,
    }

    class SwitchStateChangedEventArgs : EventArgs
    {
        public readonly SwitchState SwitchState;
        public SwitchStateChangedEventArgs(SwitchState state)
        {
            SwitchState = state;
        }
    }

    class TouchSwitch : IDisposable
    {
        private GpioPin sensor;
        private int pin;

        /// <summary>
        /// Get Switch State
        /// </summary>
        public SwitchState State
        {
            get
            {
                return ReadState();
            }
        }

        /// <summary>
        /// Create a TouchSwitch object
        /// </summary>
        /// <param name="pin">Pin number</param>
        public TouchSwitch(int pin)
        {
            this.pin = pin;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();
            sensor = gpio.OpenPin(pin);
            sensor.SetDriveMode(GpioPinDriveMode.Input);

            sensor.ValueChanged += Sensor_ValueChanged;
        }

        /// <summary>
        /// Read GpioPinValue
        /// </summary>
        /// <returns>GpioPinValue</returns>
        public GpioPinValue Read()
        {
            return sensor.Read();
        }

        /// <summary>
        /// Read SwitchState
        /// </summary>
        /// <returns>SwitchState</returns>
        private SwitchState ReadState()
        {
            var value = sensor.Read();
            switch (value)
            {
                case GpioPinValue.Low:
                    return SwitchState.OFF;
                case GpioPinValue.High:
                    return SwitchState.ON;
                default:
                    return SwitchState.OFF;
            }
        }
        
        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }

        public delegate void SwitchStateChangedHandle(object sender, SwitchStateChangedEventArgs e);
        /// <summary>
        /// Triggering when SwitchState changes
        /// </summary>
        public event SwitchStateChangedHandle SwitchStateChanged;

        private void Sensor_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            SwitchStateChanged(sender, new SwitchStateChangedEventArgs(ReadState()));
        }
    }
}
