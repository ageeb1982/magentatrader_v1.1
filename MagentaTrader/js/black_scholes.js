var black_scholes = (function () {
    return {
        P: 0.2316419,

        B1: 0.319381530,

        B2: -0.356563782,

        B3: 1.781477937,

        B4: -1.821255978,

        B5 : 1.330274429,

        // Calculate Black Scholes
        // callOption: True / False (Call / Put)
        // s: Spot price of underlying stock/asset
        // k: Strike price
        // r: Risk free annual interest rate continuously compounded (%)
        // t: Time in years until option expiration (maturity) (years)
        // v: Implied volatility of returns of underlying stock/asset (%)
        calculate: function(callOption,s,k,r,t,v) {
            if (callOption) {
                var cd1 = this.cumulativeDistribution(this.d1(s, k, r, t, v));
                var cd2 = this.cumulativeDistribution(this.d2(s, k, r, t, v));
                return s * cd1 - k * Math.exp(-r * t) * cd2;
            } else {
                var cd1 = this.cumulativeDistribution(-1 * this.d1(s, k, r, t, v));
                var cd2 = this.cumulativeDistribution(-1 * this.d2(s, k, r, t, v));
                return k * Math.exp(-r * t) * cd2 - s * cd1;
            }
        },

        d1: function (s, k, r, t, v) {
            var top = Math.log(s / k) + (r + Math.pow(v, 2) / 2) * t;
            var bottom = v * Math.sqrt(t);
            return top / bottom;
        },

        d2: function(s, k, r, t, v) {
            return this.d1(s, k, r, t, v) - v * Math.sqrt(t);
        },

        cumulativeDistribution: function(x) {
            var t = 1 / (1 + this.P * Math.abs(x));
            var t1 = this.B1 * Math.pow(t, 1);
            var t2 = this.B2 * Math.pow(t, 2);
            var t3 = this.B3 * Math.pow(t, 3);
            var t4 = this.B4 * Math.pow(t, 4);
            var t5 = this.B5 * Math.pow(t, 5);
            var b = t1 + t2 + t3 + t4 + t5;
            var cd = 1 - this.standardNormalDistribution(x) * b;
            return x < 0 ? 1 - cd : cd;
        },

        standardNormalDistribution: function(x) {
            var top = Math.exp(-0.5 * Math.pow(x, 2));
            var bottom = Math.sqrt(2 * Math.PI);
            return top / bottom;
        }

    };
}());