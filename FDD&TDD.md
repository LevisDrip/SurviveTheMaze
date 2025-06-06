# Survive the Maze | FDD & TDD

### --------------------------------------------------------------------------
# FDD

## Het spel:

Mijn spel Survive the Maze is als vast zitten in een soort labyrinth, waarin alles je probeert te doden.

Gelukkig kun jij jezelf verdedigen en de uitgang van ieder nieuw labyrinth vinden. In het labyrinth zul
je items kunnen vinden, waarmee je je tijd in het labyrinth ietsjes makkelijker kunt maken. 

Het doel:
Blijf zo lang mogelijk in leven!


## Aankomende toevoegingen:

Dit zijn de toevoegingen waaraan ik de komende tijd wil werken:

* Items die gebruikt zijn moeten verwijderd worden van de inventory.
* Granaten moeten werken.
* Een teller van vijandelijke kills moet beschikbaar zijn binnen het inventory scherm.
* Coins implementeren, Mocht er tijd over zijn.
* Mogelijk met een tekort aan tijd: Meer enemies


### --------------------------------------------------------------------------

# TDD

## The game

In my game, Survive the maze, you will be thrown in a randomly generated room where monsters will try to kill you.

Your one and only goal is to survive and find the exit to each maze room.

The maze rooms are generated using a noise texture. By using a navMeshAgent, I was able to generate a path that will
always lead to a possible exit. 

During your time in the maze, you'll encounter enemies and resources that can be used to your advantage.
Items like pills and grenades are to be used to help defeat enemies quicker or heal you in combat.

In your inventory screen, you'll find all the items you have collected thus far. You will also be able to see
how many enemies you have killed, and the amount of coins you have collected. Every once in a while you will
encounter a merchant before going to the next maze. You will be able to use the coins to buy items or new weapons.
(Plans for later, will not implement in this class. Hope to implement this in the near future.)

You can equip items by clicking on them and then pressing equip. You may also discard items, by doing the same thing,
except instead of pressing equip, you press discard. You will start out with a simple gun. (may implement more guns in the future.)

You have to watch your ammo though. Cause if you run out, then you will be dead in a matter of seconds. (don't have ammo yet
but might implement later as well.)


## Upcoming additions

* Used items are to be deleted from the inventory UI. When consuming items, like grenades or pills, they should be removed from the
  inventory UI. However, this is currently broken. This is the first thing I will fix.
* Grenades should be able to work. Right now, grenades are purely for show. I want to be able to throw them and deal damage to enemies with them.
* An enemy kill counter in the inventory UI. The inventory UI should have an enemy kill counter.
* (If possible with time) More enemies. Right now, there is only one enemy. However, I would like to add more, so the game has a bit more variation.
* (If possible with time) Implement coins as a mechanic. As of now it would be only to collect coins. They will not have a use quite yet.
