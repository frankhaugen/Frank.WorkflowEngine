namespace Frank.WorkflowEngine.Tests
{
    public class IdHelperTests(ITestOutputHelper outputHelper)
    {
        private readonly ITestOutputHelper _outputHelper = outputHelper;

        [Fact]
        public void CreateWorkflowId_ShouldGenerateNewGuid()
        {
            // Act
            var id = IdHelper.CreateWorkflowId();

            // Assert
            id.Should().NotBe(Guid.Empty);
            _outputHelper.WriteLine(id.ToString());
        }

        [Fact]
        public void GetJobId_ShouldGenerateNewGuid()
        {
            // Act
            var id = IdHelper.GetJobId();

            // Assert
            id.Should().NotBe(Guid.Empty);
            _outputHelper.WriteLine(id.ToString());
        }

        [Fact]
        public void GetStepId_ShouldGenerateNewGuid()
        {
            // Act
            var id = IdHelper.GetStepId();

            // Assert
            id.Should().NotBe(Guid.Empty);
            _outputHelper.WriteLine(id.ToString());
        }

        [Fact]
        public void CombineGuids_ShouldCombineMultipleGuids()
        {
            // Arrange
            var guid1 = IdHelper.CreateWorkflowId();
            var guid2 = IdHelper.GetJobId();

            // Act
            var combinedGuid = IdHelper.CombineGuids(guid1, guid2);

            // Assert
            combinedGuid.Should().NotBe(guid1);
            combinedGuid.Should().NotBe(guid2);
            _outputHelper.WriteLine(guid1.ToString());
            _outputHelper.WriteLine(guid2.ToString());
        }

        [Fact]
        public void CombineGuids_EmptyInput_ShouldReturnEmptyGuid()
        {
            // Act
            var combinedGuid = IdHelper.CombineGuids();

            // Assert
            combinedGuid.Should().Be(Guid.Empty);
            _outputHelper.WriteLine(combinedGuid.ToString());
        }

        [Fact]
        public void CombineGuids_SingleGuid_ShouldReturnSameGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var combinedGuid = IdHelper.CombineGuids(guid);

            // Assert
            combinedGuid.Should().Be(guid);
            _outputHelper.WriteLine(guid.ToString());
            _outputHelper.WriteLine(combinedGuid.ToString());
        }

        [Fact]
        public void CombineGuids_DuplicateGuids_ShouldReturnSameGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var combinedGuid = IdHelper.CombineGuids(guid, guid);

            // Assert
            combinedGuid.Should().Be(guid);
            _outputHelper.WriteLine(combinedGuid.ToString());
        }
        
        [Fact]
        public void CombineGuids_ThreeGuids_ShouldCombineGuids()
        {
            // Arrange
            var guid1 = IdHelper.CreateWorkflowId();
            var guid2 = IdHelper.GetJobId();
            var guid3 = IdHelper.GetStepId();

            // Act
            var combinedGuid = IdHelper.CombineGuids(guid1, guid2, guid3);

            // Assert
            combinedGuid.Should().NotBe(guid1);
            combinedGuid.Should().NotBe(guid2);
            combinedGuid.Should().NotBe(guid3);
            _outputHelper.WriteLine(guid1.ToString());
            _outputHelper.WriteLine(guid2.ToString());
            _outputHelper.WriteLine(guid3.ToString());
            _outputHelper.WriteLine(combinedGuid.ToString());
            var ids = new Ids(guid1, guid2, guid3, combinedGuid);
            _outputHelper.WriteLine(ids.ToString());
        }
        
        private record Ids(Guid WorkflowId, Guid JobId, Guid StepId, Guid CombinedGuid);
    }
}