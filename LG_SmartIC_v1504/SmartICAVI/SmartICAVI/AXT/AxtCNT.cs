using System;
using System.Runtime.InteropServices;


public class AxtCNT
{
    [DllImport("AxtLib.dll")]
    public static extern int InitializeCNT(int reset);
    [DllImport("AxtLib.dll")]
    public static extern int CNTIsInitialized();

    // Unit/Pulse 설정/확인한다.
	[DllImport("AxtLib.dll")]
    public static extern int CNTset_moveunit_perpulse(short channel, double unitperpulse);
    // Encoder입력 방식을 설정한다.
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_enc_input_method(short channel, byte method);
    // Encoder 입력 반전여부를 설정한다.
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_enc_reverse(short channel, byte reverse);
    // Encoder 입력 신호형태를 설정한다.
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_enc_source_sel(short channel, byte phase);
    // 트리거 출력 모드를 설정함.(0: PERIODIC POSITION EDGE, 1: PERIODIC TIME TRIGGER)
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_mode(short channel, byte mode);
	// 트리거 영역의 하한값/상항값/주기를 설정함.
	[DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_block(short channel, double dLower, double dUpper, double dPeriod);
    // 트리거 출력신호의 펄스폭을 설정함 [usec], ex) pulse_width = 10, 10usec, 0.02use ~ 335544.32usec
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_pulse_width(short channel, double pulse_width);
    // 트리거 출력신호의 Active Level을 설정함 (0: Active Low, 1: Active High)
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_active_level(short channel, byte level);
    // 트리거 유효방향 설정함.(0: 트리거 방향설정기능 사용안함, 1: CW방향트리거 설정, 2: CCW방향트리거 설정)
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_direction_check(short channel, byte uDirection);
    // 펄스 카운트 값을 설정합니다.
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_pulse_counter(short channel, double count);
    // 트리거를 ENABLE한다.
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_enable(short channel);
    [DllImport("AxtLib.dll")]
    public static extern int CNTset_trigger_disable(short channel);

}
