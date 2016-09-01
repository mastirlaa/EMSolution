namespace EMLibrary.Tests
{
    public class MockNameProvider : EMLibrary.INameProvider
    {
        private string m_mockName;

        public MockNameProvider(string mockName)
        {
            m_mockName = mockName;
        }

        public void replaceMockName(string newMockName)
        {
            m_mockName = newMockName;
        }

        string EMLibrary.INameProvider.Name
        {
            get
            {
                return m_mockName;
            }
        }
    }
}
