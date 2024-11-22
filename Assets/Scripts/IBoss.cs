using System;
using System.Collections;

public interface IBoss
{
    public void PhaseChange(int bossPhase);
    public void Phase1();
    public void Phase2();
    public void Phase3();
    public void Phase4();
    public void Phase5();
}
