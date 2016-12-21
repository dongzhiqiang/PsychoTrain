
public class TimeCheck
{
    long time;

    public TimeCheck(bool beginNow = true)
    {
        time = 0;
        if (beginNow == true)
            begin();
    }

    public void begin()
    {
        
        time = System.DateTime.Now.Ticks;
        
    }

    public float delay
    {
        get
        {
            return (float)((System.DateTime.Now.Ticks - time) / (double)System.TimeSpan.TicksPerSecond);
        }
    }

    public float delayMS
    {
        get
        {
            return (float)((System.DateTime.Now.Ticks - time)*1000 / (double)System.TimeSpan.TicksPerSecond);
        }
    }


    // ��ȡ�ϴ�ʱ�䣬�����¿�ʼ��ʱ
    public float renew
    {
        get
        {
            float t = delay;
            begin();
            return t;
        }
    }
}
