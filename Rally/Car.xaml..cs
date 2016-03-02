using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rally
{
   
        public sealed partial class Car : UserControl
        {
            #region Private Properties 

            public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(double), typeof(Car), new PropertyMetadata(0.0));

            private double _acceleration = 2;
            private double _rotationStep = 8;
            private double _speed = 0;
            private double _speedDecay = 0.9;
            private double _maxSpeed = 12;
            private double _backSpeed = 1.2;

            private bool upKeyDown, downKeyDown, leftKeyDown, rightKeyDown;

            #endregion

            #region Constructor 

            public Car()
            {
                this.InitializeComponent();

                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
            {
                switch (args.VirtualKey)
                {
                    case Windows.System.VirtualKey.Up:
                        upKeyDown = false;
                        break;
                    case Windows.System.VirtualKey.Down:
                        downKeyDown = false;
                        break;
                    case Windows.System.VirtualKey.Left:
                        leftKeyDown = false;
                        break;
                    case Windows.System.VirtualKey.Right:
                        rightKeyDown = false;
                        break;
                }
            }

            private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
            {
                switch (args.VirtualKey)
                {
                    case Windows.System.VirtualKey.Up:
                        upKeyDown = true;
                        break;
                    case Windows.System.VirtualKey.Down:
                        downKeyDown = true;
                        break;
                    case Windows.System.VirtualKey.Left:
                        leftKeyDown = true;
                        break;
                    case Windows.System.VirtualKey.Right:
                        rightKeyDown = true;
                        break;
                }
            }

            #endregion

            #region Public Properties 

            public double Heading
            {
                get
                {
                    return (double)GetValue(HeadingProperty);
                }
                set
                {
                    SetValue(HeadingProperty, value);
                }
            }

            private double _scale = 1;
            public double Scale
            {
                get { return _scale; }
                set
                {
                    _scale = value;
                }
            }

            #endregion

            #region Public Methods 

            public Point GetPositionOffset()
            {
                if (upKeyDown)
                {
                    Accelerate();
                }

                if (downKeyDown)
                {
                    Decelerate();
                }

                if (leftKeyDown)
                {
                    SteerLeft();
                }

                if (rightKeyDown)
                {
                    SteerRight();
                }

                if (!IsMoving())
                {
                    _speed = 0;
                }
                else if (!upKeyDown || !downKeyDown)
                {
                    _speed *= _speedDecay;
                }

                if (_speed != 0)
                {
                    var hRadian = Heading * Math.PI / 180;
                    var dx = Math.Sin(hRadian) * _speed;
                    var dy = Math.Cos(hRadian) * _speed * -1;

                    return new Point(dx, dy);
                }

                return new Point(0, 0);
            }

            #endregion

            #region Private Methods 

            private bool IsMoving()
            {
                return !(_speed > -0.4 && _speed < 0.4);
            }

            private void Accelerate()
            {
                if (_speed < _maxSpeed)
                {
                    if (_speed < 0)
                    {
                        _speed *= _speedDecay;
                    }
                    else if (_speed == 0)
                    {
                        _speed = 0.4;
                    }
                    else {
                        _speed *= _acceleration;
                    }
                }
            }

            private void Decelerate()
            {
                if (Math.Abs(_speed) < _maxSpeed)
                {
                    if (_speed > 0)
                    {
                        _speed *= _speedDecay;
                        _speed = (_speed < 0) ? 0 : _speed;
                    }
                    else if (_speed == 0)
                    {
                        _speed = -0.4;
                    }
                    else
                    {
                        _speed *= _backSpeed;
                        _speed = (_speed > 0) ? 0 : _speed;
                    }
                }
            }

            private void SteerLeft()
            {
                if (IsMoving())
                {
                    Heading -= _rotationStep * (_speed / _maxSpeed);
                }
            }

            private void SteerRight()
            {
                if (IsMoving())
                {
                    Heading += _rotationStep * (_speed / _maxSpeed);
                }
            }

            #endregion
        }
    }

