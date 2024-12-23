using System;
using Xunit;
using Sabatex.Extensions;

namespace Sabatex.Extensions.Tests;

 public class UInt128Tests
 {
     [Fact]
     public void InitialUInt128()
     {
         UInt128 a = 0;
         UInt128 b = 10;
         a = long.MaxValue;
        
     }
    [Fact]
     public void EqualsUInt128()
     {
         UInt128 a = 0;
         UInt128 b = 0;
         Assert.Equal(a,b);
         a = UInt128.MaxValue;
         b = UInt128.MaxValue;
         Assert.Equal(a,b);
         b = UInt128.MinValue;
         Assert.NotEqual(a,b);
        
     }





 }
