namespace Project.Web.Shared.Test
{
    public partial class ErrorCatcherTest
    {
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                //throw new Exception("测试");
            }
        }
    }
}
