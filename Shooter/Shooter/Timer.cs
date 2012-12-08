using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class Timer
    {
        double resetTime;
        double timePeriod;

        /// <summary>
        /// Creates a new Timer object
        /// </summary>
        /// <param name="timePeriod">The amount of time to wait every period</param>
        public Timer(int timePeriod)
        {
            this.timePeriod = timePeriod;
            this.resetTime = 0;
        }

        /// <summary>
        /// Sets the time period
        /// </summary>
        /// <param name="timePeriod"></param>
        public void setPeriod(int timePeriod)
        {
            this.timePeriod = timePeriod;
        }

        /// <summary>
        /// Should be called in the XNA Update() method. Returns true if it's waited enough
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public bool update(GameTime gameTime)
        {
            double now = gameTime.TotalGameTime.TotalMilliseconds;

            if (now - resetTime >= timePeriod)
            {
                reset(now);
                return true;
            }

            return false;
        }

        void reset(double resetTime)
        {
            this.resetTime = resetTime;
        }
    }
}
