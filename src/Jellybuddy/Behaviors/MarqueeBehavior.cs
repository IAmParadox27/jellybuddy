using System.Diagnostics;
using Syncfusion.Maui.Toolkit.Buttons;

namespace Jellybuddy.Behaviors
{
    public class MarqueeBehavior : Behavior<StackLayout>
    {
        private StackLayout? m_stack = null;
        private readonly IDispatcherTimer m_timer;
        private double m_pauseDurationRemaining;
        private FlowDirection m_previousDirection;
        private Stopwatch m_pauseStopwatch = new Stopwatch();

        public static readonly BindableProperty PageWidthProperty = 
            BindableProperty.Create(nameof(PageWidth), typeof(double), typeof(MarqueeBehavior));

        public double PageWidth
        {
            get { return (double)GetValue(PageWidthProperty); }
            set { SetValue(PageWidthProperty, value); }
        }

        public static readonly BindableProperty ScrollOverflowProperty = 
        BindableProperty.Create(nameof(ScrollOverflow), typeof(double), typeof(MarqueeBehavior));

        public double ScrollOverflow
        {
            get { return (double)GetValue(ScrollOverflowProperty); }
            set { SetValue(ScrollOverflowProperty, value); }
        }

        public static readonly BindableProperty SpeedProperty = 
            BindableProperty.Create(nameof(Speed), typeof(double), typeof(MarqueeBehavior), (double)5);

        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly BindableProperty DirectionChangePauseDurationProperty = 
            BindableProperty.Create(nameof(DirectionChangePauseDuration), typeof(double), typeof(MarqueeBehavior), (double)1);

        public double DirectionChangePauseDuration
        {
            get { return (double)GetValue(DirectionChangePauseDurationProperty); }
            set { SetValue(DirectionChangePauseDurationProperty, value); }
        }

        public FlowDirection Direction { get; set; } = FlowDirection.LeftToRight;

        public double StackContentWidth
        {
            get
            {
                if (m_stack == null)
                {
                    return 0;
                }

                double width = 0;
                foreach (View child in m_stack.Children.OfType<View>())
                {
                    width += child.Width;
                }
                
                return width;
            }
        }
        
        public MarqueeBehavior()
        {
            m_timer = Application.Current.Dispatcher.CreateTimer();
            m_timer.Interval = TimeSpan.FromMilliseconds(50);
            m_timer.Tick += Timer_OnTick;
        }

        protected override void OnAttachedTo(StackLayout stackLayout)
        {
            base.OnAttachedTo(stackLayout);
            m_stack = stackLayout;
            
            StartAnimation();
        }

        protected override void OnDetachingFrom(StackLayout bindable)
        {
            base.OnDetachingFrom(bindable);
            m_stack = null;
        }

        /// <summary>
        /// This method is used for starting the marquee scrolling animation.
        /// </summary>
        private void StartAnimation()
        {
            m_pauseDurationRemaining = DirectionChangePauseDuration;
            m_timer.Start();
        }

        private void Timer_OnTick(object? sender, EventArgs e)
        {
            if (m_stack == null)
            {
                m_timer.Stop();
                return;
            }

            if (StackContentWidth < PageWidth)
            {
                // There's no point in this behavior if the stack is smaller than the page.
                m_timer.Stop();
                return;
            }

            if (Direction != FlowDirection.MatchParent)
            {
                m_stack.TranslationX -= Direction == FlowDirection.LeftToRight ? Speed : -Speed;
                
                if (Direction == FlowDirection.LeftToRight)
                {
                    double leftThreshold = (StackContentWidth - PageWidth) + ScrollOverflow;
                    if (Math.Abs(m_stack.TranslationX) > leftThreshold)
                    {
                        m_previousDirection = FlowDirection.LeftToRight;
                        m_pauseStopwatch.Start();
                        Direction = FlowDirection.MatchParent;
                    }
                }
                else if (Direction == FlowDirection.RightToLeft)
                {
                    if (m_stack.TranslationX > 0)
                    {
                        m_previousDirection = FlowDirection.RightToLeft;
                        m_pauseStopwatch.Start();
                        Direction = FlowDirection.MatchParent;
                    }
                }
            }
            else
            {
                if (m_pauseStopwatch.Elapsed.TotalSeconds >= DirectionChangePauseDuration)
                {
                    m_pauseStopwatch.Stop();
                    m_pauseStopwatch.Reset();
                    Direction = m_previousDirection == FlowDirection.LeftToRight ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                }
            }
        }
    }
}