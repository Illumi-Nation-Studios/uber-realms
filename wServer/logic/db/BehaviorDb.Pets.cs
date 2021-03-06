﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.logic.attack;
using wServer.logic.movement;
using wServer.logic.loot;
using wServer.logic.taunt;
using wServer.logic.cond;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        static _ Pets = Behav()
            .InitMany(0x1642, 0x1660, Behaves("Pet",
                If.Instance(new PetBehaves(), PetChasing.Instance(9, 5, 1)),
                    Cooldown.Instance(1750, new PetHealHpMp()),
                    Cooldown.Instance(1500, new PetAttack(50, 80, 10))
            ));
    }
}