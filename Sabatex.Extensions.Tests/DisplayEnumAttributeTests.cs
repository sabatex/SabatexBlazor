using Sabatex.Extensions.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Xunit;

namespace Sabatex.Extensions.Tests;

public class DisplayEnumAttributeTests
{


    [Fact]
    public void Ctor()
    {
        DisplayEnumAttribute attribute = new DisplayEnumAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.GetName());

        Assert.Null(attribute.ResourceType);
    }

    public static IEnumerable<object[]> Strings_TestData()
    {
        yield return new object[] { "" };
        yield return new object[] { " \r \t \n " };
        yield return new object[] { "abc" };
    }

    [Theory]
    [MemberData(nameof(Strings_TestData))]
    [InlineData("Name")]
    public void Name_Get_Set(string value)
    {
        DisplayEnumAttribute attribute = new DisplayEnumAttribute();
        attribute.Name = value;

        Assert.Equal(value, attribute.Name);
        Assert.Equal(value == null, attribute.GetName() == null);

        // Set again, to cover the setter avoiding operations if the value is the same
        attribute.Name = value;
        Assert.Equal(value, attribute.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(typeof(string))]
    public void ResourceType_Get_Set(Type value)
    {
        DisplayEnumAttribute attribute = new DisplayEnumAttribute();
        attribute.ResourceType = value;
        Assert.Equal(value, attribute.ResourceType);

        // Set again, to cover the setter avoiding operations if the value is the same
        attribute.ResourceType = value;
        Assert.Equal(value, attribute.ResourceType);
    }



}
