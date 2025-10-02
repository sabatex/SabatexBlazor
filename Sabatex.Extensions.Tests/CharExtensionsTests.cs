using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Sabatex.Extensions.ClassExtensions;

namespace Sabatex.Extensions.Tests;

public class CharExtensionsTests
{
    [Theory]
    [InlineData(@"`qwertyuiop[]asdfghjkl;'zxcvbnm,.")]//eng
    [InlineData(@"ёйцукенгшщзхъфывапролджэячсмитьбю")]//rus
    public void UpperKeyToCultureUkMethod(string testData)
    {
        string etalon = @"ЁЙЦУКЕНГШЩЗХЇФІВАПРОЛДЖЄЯЧСМИТЬБЮ";
        Assert.Equal(testData.ToUkrainian(), etalon);

    }

    [Theory]
    [InlineData(@"`qwertyuiop[]asdfghjkl;'zxcvbnm,.")]//eng
    [InlineData(@"ЁЙЦУКЕНГШЩЗХЇФІВАПРОЛДЖЄЯЧСМИТЬБЮ")]//ukr
    public void UpperKeyToCultureRusMethod(string testData)
    {
        string etalon = @"ёйцукенгшщзхъфывапролджэячсмитьбю".ToUpper();
        Assert.Equal(testData.ToRussian(), etalon);
    }

}
