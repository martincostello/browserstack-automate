// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
#if NET451
using System.Runtime.Serialization;
#endif

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// Represents an error from the BrowserStack Automate API.
    /// </summary>
#if NET451
    [Serializable]
#endif
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
        public BrowserStackAutomateException(BrowserStackAutomateError errorDetail)
            : base(errorDetail?.Message ?? DefaultMessage)
        {
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with a specified error message and error detail.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorDetail">The error detail, if any.</param>
        public BrowserStackAutomateException(string message, BrowserStackAutomateError errorDetail)
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

#if NET451
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is <see langword="null"/>.
        /// </exception>
        protected BrowserStackAutomateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorDetail = (BrowserStackAutomateError)info.GetValue(nameof(ErrorDetail), typeof(BrowserStackAutomateError));
        }
#endif

        /// <summary>
        /// Gets the error detail, if any, associated with the exception.
        /// </summary>
        public BrowserStackAutomateError ErrorDetail { get; }

#if NET451
        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ErrorDetail), ErrorDetail);
        }
#endif
    }
}
