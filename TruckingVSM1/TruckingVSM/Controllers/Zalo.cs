using Newtonsoft.Json.Linq;
using ZaloDotNetSDK;

namespace TruckingVSM.Controllers
{
    public class Zalo
    {
        public int sendMessage(string UserId, string Message)
        {
            ZaloClient client = new ZaloClient("0__XHlbWJHG2_-P9yMTA6Wdzw4BrV3z4QUlhLTGMFKzBZUX0zXSN7cw1s2_FLm8mMvF8BwLl6J00aT0oZszv52ArfnwJQqu_CRwO9BrdLYiHe_Goe7yw3WcWvXot8Zi64kAK3hSCVoipoPmEe20s3aRMvnJbE3epQiFJ8V412mzovji0_puo2d_ed1dM0rK2Ki-OMSOIMsL9qxPNun1gIK_vl4_q1M5BLVEGIz8_Creeflr_kdmV9IQhz2-OHLy2C9hq6Ava8ZetkECVh7GA6Y-tsa7Bhvjh-tvB70");
            JObject result = client.sendTextMessageToUserId(UserId, Message);
            return result.Count;
        }
    }
}