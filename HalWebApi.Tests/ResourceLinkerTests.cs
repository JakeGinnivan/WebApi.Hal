using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace HalWebApi.Tests
{
    public class ResourceLinkerTests
    {
        [Fact]
        [UseReporter(typeof(DiffReporter))] 
        public void throws_meaningful_exception_when_cannot_find_linker()
        {
            // arrange
            var resourceLinker = new ResourceLinker();
            Exception exception = null;

            // act
            try
            {
                resourceLinker.CreateLinks(new List<HalResource>());
            }
            catch (ArgumentException ex)
            {
                exception = ex;
            }

            // assert
            Approvals.Verify(exception.Message);
        }
    }
}