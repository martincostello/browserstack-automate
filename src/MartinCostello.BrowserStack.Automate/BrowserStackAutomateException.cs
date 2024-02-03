// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// Represents an error from the BrowserStack Automate API.
    /// </summary>
    public class BrowserStackAutomateException : Exception
    {
        /// <summary>
        /// The default error message.
        /// </summary>
        private const string DefaultMessage = "A BrowserStack Automate error occurred.";

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class.
        /// </summary>
        public BrowserStackAutomateException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BrowserStackAutomateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified error message.
        /// </summary>
        /// <param name="errorDetail">The error detail, if any.</param>
        public BrowserStackAutomateException(BrowserStackAutomateError? errorDetail)
            : base(errorDetail?.Message ?? DefaultMessage)
        {
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified error message and error detail.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorDetail">The error detail, if any.</param>
        public BrowserStackAutomateException(string message, BrowserStackAutomateError? errorDetail)
            : base(message)
        {
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.
        /// </param>
        public BrowserStackAutomateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified error message and error detail.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorDetail">The error detail, if any.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.
        /// </param>
        public BrowserStackAutomateException(string message, BrowserStackAutomateError errorDetail, Exception innerException)
            : base(message, innerException)
        {
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Gets the error detail, if any, associated with the exception.
        /// </summary>
        public BrowserStackAutomateError? ErrorDetail { get; }
    }
}
