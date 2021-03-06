﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;
using Mono.Game;
using wServer.logic;

namespace wServer.logic
{
    class PetAttack : Behavior
    {

        int lowest;
        int highest;
        float dist;
        DamageCounter counter;

        public PetAttack(int lowest, int highest, float dist)
        {
            this.lowest = lowest;
            this.highest = highest;
            this.dist = dist;
        }

        protected override bool TickCore(RealmTime time)
        {
            float radius = dist;
            Enemy entity = GetNearestEntityPet(ref radius) as Enemy;

            if (entity != null)
            {
                entity.Owner.BroadcastPacket(new ShowEffectPacket()
                {
                    EffectType = EffectType.Potion,
                    TargetId = entity.Id,
                    Color = new ARGB(0xFF0000)
                }, null);
                entity.Owner.BroadcastPacket(new ShowEffectPacket()
                {
                    EffectType = EffectType.Trail,
                    TargetId = Host.Self.Id,
                    PosA = new Position() { X = entity.X, Y = entity.Y },
                    Color = new ARGB(0xFF0000)
                }, null);
                entity.Damage(Host.Self.PlayerOwner, time, new Random().Next(lowest, highest), false, new ConditionEffect[] { });
            }
            return false;
        }
    }
    class PetAttackRadius : Behavior
    {

        int lowest;
        int highest;
        float radius;
        DamageCounter counter;

        public PetAttackRadius(int lowest, int highest, float radius)
        {
            this.lowest = lowest;
            this.highest = highest;
            this.radius = radius;
        }

        protected override bool TickCore(RealmTime time)
        {
            float dist = 10;
            Enemy entity = GetNearestEntityPet(ref dist) as Enemy;


            if (entity != null)
            {
                var distance = Vector2.Distance(new Vector2(Host.Self.X, Host.Self.Y), new Vector2(entity.X, entity.Y));
                var enemies = new List<Enemy>();

                Behavior.AOE(entity.Owner, entity, radius, false, enemy =>
                {
                    enemies.Add(enemy as Enemy);

                });
                if (distance < 10)
                {
                    foreach (var enemy in enemies)
                    {
                        int hp = enemy.HP;
                        if (entity.HasConditionEffect(ConditionEffects.Invulnerable) == false)
                        {
                            hp = Math.Min(hp - (new Random().Next(lowest, highest) - entity.ObjectDesc.Defense), entity.ObjectDesc.MaxHP);





                            if (hp != enemy.HP)
                            {
                                int n = hp - enemy.HP;
                                entity.HP = hp;
                                entity.UpdateCount++;

                                entity.Owner.BroadcastPacket(new ShowEffectPacket()
                                {
                                    EffectType = EffectType.Potion,
                                    TargetId = entity.Id,
                                    Color = new ARGB(0xFF0000)
                                }, null);
                                entity.Owner.BroadcastPacket(new ShowEffectPacket()
                                {
                                    EffectType = EffectType.Trail,
                                    TargetId = Host.Self.Id,
                                    PosA = new Position() { X = entity.X, Y = entity.Y },
                                    Color = new ARGB(0xFF0000)
                                }, null);
                                entity.Owner.BroadcastPacket(new ShowEffectPacket()
                                {
                                    EffectType = EffectType.Diffuse,
                                    Color = new ARGB(0xFF0000),
                                    TargetId = Host.Self.Id,
                                    PosA = new Position() { X = entity.X, Y = entity.Y },
                                    PosB = new Position() { X = entity.X + radius, Y = entity.Y }
                                }, null);
                                entity.Owner.BroadcastPacket(new NotificationPacket()
                                {
                                    ObjectId = entity.Id,
                                    Text = "" + n,
                                    Color = new ARGB(0xFF0000)
                                }, null);



                                if (entity.HP < 0)
                                {
                                    foreach (var i in entity.CondBehaviors)
                                        if ((i.Condition & BehaviorCondition.OnDeath) != 0)

                                            i.Behave(BehaviorCondition.OnDeath, entity, time, counter);

                                    if (entity != null)
                                        entity.Owner.LeaveWorld(entity);

                                }

                            }
                        }
                        return true;
                    }
                }

            }
            return false;
        }
    }
}