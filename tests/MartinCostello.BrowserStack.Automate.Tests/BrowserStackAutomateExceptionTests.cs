// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

public static class BrowserStackAutomateExceptionTests
{
    [Fact]
    public static void Default_Constructor_Initializes_Properties()
    {
        // Act
        var exception = new BrowserStackAutomateException();

        // Assert
        exception.Message.ShouldBe("A BrowserStack Automate error occurred.");
        exception.ErrorDetail.ShouldBeNull();
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_Message_Initializes_Properties()
    {
        // Arrange
        string expectedMessage = "Test message";

        // Act
        var exception = new BrowserStackAutomateException(expectedMessage);

        // Assert
        exception.Message.ShouldBe(expectedMessage);
        exception.ErrorDetail.ShouldBeNull();
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_Message_And_InnerException_Initializes_Properties()
    {
        // Arrange
        string expectedMessage = "Test message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new BrowserStackAutomateException(expectedMessage, innerException);

        // Assert
        exception.Message.ShouldBe(expectedMessage);
        exception.ErrorDetail.ShouldBeNull();
        exception.InnerException.ShouldBeSameAs(innerException);
    }

    [Fact]
    public static void Constructor_With_Null_ErrorDetail_Initializes_Properties()
    {
        // Arrange
        BrowserStackAutomateError? errorDetail = null;

        // Act
        var exception = new BrowserStackAutomateException(errorDetail);

        // Assert
        exception.Message.ShouldBe("A BrowserStack Automate error occurred.");
        exception.ErrorDetail.ShouldBeNull();
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_Null_ErrorDetail_Message_Initializes_Properties()
    {
        // Arrange
        var errorDetail = new BrowserStackAutomateError() { Message = null! };

        // Act
        var exception = new BrowserStackAutomateException(errorDetail);

        // Assert
        exception.Message.ShouldBe("A BrowserStack Automate error occurred.");
        exception.ErrorDetail.ShouldBeSameAs(errorDetail);
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_ErrorDetail_Initializes_Properties()
    {
        // Arrange
        var errorDetail = new BrowserStackAutomateError()
        {
            Message = "Test error message",
        };

        // Act
        var exception = new BrowserStackAutomateException(errorDetail);

        // Assert
        exception.Message.ShouldBe(errorDetail.Message);
        exception.ErrorDetail.ShouldBeSameAs(errorDetail);
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_Message_And_ErrorDetail_Initializes_Properties()
    {
        // Arrange
        string expectedMessage = "Test message";
        var errorDetail = new BrowserStackAutomateError()
        {
            Message = "Test error message",
        };

        // Act
        var exception = new BrowserStackAutomateException(expectedMessage, errorDetail);

        // Assert
        exception.Message.ShouldBe(expectedMessage);
        exception.ErrorDetail.ShouldBeSameAs(errorDetail);
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public static void Constructor_With_Message_Inner_Exception_And_ErrorDetail_Initializes_Properties()
    {
        // Arrange
        string expectedMessage = "Test message";
        var innerException = new InvalidOperationException("Inner exception");
        var errorDetail = new BrowserStackAutomateError()
        {
            Message = "Test error message",
        };

        // Act
        var exception = new BrowserStackAutomateException(expectedMessage, errorDetail, innerException);

        // Assert
        exception.Message.ShouldBe(expectedMessage);
        exception.ErrorDetail.ShouldBeSameAs(errorDetail);
        exception.InnerException.ShouldBeSameAs(innerException);
    }
}
