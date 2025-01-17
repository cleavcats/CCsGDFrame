

namespace CCsGDFrame
{
    public partial class UI_Debug
    {
        public void When_一键过关()
        {
            Demo.GamingData.ptr.map[0][0] = 1;
            Demo.GamingData.ptr.map[1][0] = 2;
            Demo.GamingData.ptr.map[2][0] = 0;
            Demo.GamingData.ptr.map[0][1] = 3;
            Demo.GamingData.ptr.map[1][1] = 4;
            Demo.GamingData.ptr.map[2][1] = 5;
            Demo.GamingData.ptr.map[0][2] = 6;
            Demo.GamingData.ptr.map[1][2] = 7;
            Demo.GamingData.ptr.map[2][2] = 8;
            Demo.GamingData.ptr.block_x = 2;
            Demo.GamingData.ptr.block_y = 0;
            UI_Close();
        }
    }
}