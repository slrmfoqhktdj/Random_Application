using System.Collections.Generic;

public static class GlobalVal
{
    //현재 뽑은 볼 갯수
    public static int getBallCount = 0;

    //뽑아야 하는 볼 갯수
    public static int getGoalBallCount = 7;

    //뽑은 볼 숫자
    public static List<int> getBallNumber = new List<int>();

    //뽑은 보너스 볼 숫자
    public static int getBallBonusNumber = 0;

    //WindZone 작동 여부
    public static bool WindZoneBool = false;

    //저장 할 수 있는 숫자 제한 
    public static int getBallListCount = 10;

    //광고 나오는 카운트
    public static int getADCount = 3;

    //시작시 공바로 들어가는 것 방지 시간
    public static float getShieldWaitTime = 3.0f;
}
