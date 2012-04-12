using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DreadWyrm2
{
    public class PreySpawner
    {

        //A list of animals to be spawned
        public static List<Prey> animals;
        public static int numAnimals;

        //A list of humans to be spawned
        public static List<Prey> humans;
        public static int numHumans;

        //A list of things to be spawned RIGHT NOW
        public static List<Prey> immediates;
        public static int numImmediates;

        //Finally, a list of prey to be removed, because the spawner just works that way
        public static List<Prey> removals;
        public static int numRemovals;


        //arbitrary spawn delays and timers
        const float ANIMAL_SPAWN_DELAY = 7;
        static float ANIMAL_SPAWN_ELAPSED = 0;

        const float HUMAN_SPAWN_DELAY = 5;
        static float HUMAN_SPAWN_ELAPSED = 0;

        //if we go to spawn animals X times, and it doesnt happen, we better just make one for balancing
        int limitNoAnimals = 1;
        int timesNoAnimals = 0;

        const int MEAT_SPAWN_LIMIT = 500;

        //The constructor
        public PreySpawner()
        {
            //The constructor initializes the lists
            animals = new List<Prey>();
            humans = new List<Prey>();
            immediates = new List<Prey>();
            removals = new List<Prey>();

            //and the counts
            numAnimals = 0;
            numHumans = 0;
            numImmediates = 0;
            numRemovals = 0;
        }

        //the update method gets a gametime, and determines if its time to spawn something
        public void update(GameTime gametime)
        {
            #region Deal with removals first

            if (numRemovals > 0)
            {
                //Now to remove
                for (int i = 0; i < numRemovals; i++)
                {
                    //get the removal
                    Prey p = removals.ElementAt(i);

                    //And remove it from the list of removables
                    removals.Remove(p);
                    //also the list of prey
                    Prey.prey.Remove(p);
                    numRemovals = numRemovals - 1;
                }
            }

            #endregion

            #region Spawn dem Immediates

            //Lets check the count of things in the immediates
            if (numImmediates > 0)
            {
                //first add them
                for (int i = 0; i < numImmediates; i++)
                {
                    //get the immediate
                    Prey p = immediates.ElementAt(i);

                    //Add it to the prey list (making it "real")
                    Prey.prey.Add(p);
                }

                //Now to remove
                for (int i = 0; i < numImmediates; i++)
                {
                    //get the immediate
                    Prey p = immediates.ElementAt(i);

                    //And remove it from the list of spawnables
                    immediates.Remove(p);
                    numImmediates = numImmediates - 1;
                }
            }

            #endregion

            #region Spawning Animals

            //Now lets deal with the animals
            //Increment the timer...
            ANIMAL_SPAWN_ELAPSED = ANIMAL_SPAWN_ELAPSED + (float)gametime.ElapsedGameTime.TotalSeconds;

            //Is it time to spawn an animal?
            if (ANIMAL_SPAWN_ELAPSED > ANIMAL_SPAWN_DELAY)
            {
                //Yes it is!
                //Reset the timer
                ANIMAL_SPAWN_ELAPSED = 0;
                
                //but wait! Are there actually any animals?
                if (numAnimals > 0)
                {
                    //turns out yes
                    Prey p = animals.ElementAt(0);

                    //Add it to the prey list (making it "real")
                    Prey.prey.Add(p);
                    //And remove it from the list of spawnables
                    animals.Remove(p);
                    numAnimals = numAnimals - 1;

                    #region dealing with spawning safari animals every once in a while
                    //Then reset the "no animals" counter
                    timesNoAnimals = 0;
                }
                else
                {
                    //No animals to spawn? is it time that we do somethign about that?
                    timesNoAnimals = timesNoAnimals + 1;
                    if ((timesNoAnimals > limitNoAnimals) && (!tooMuchMeat()))
                    {
                        int theNumber = Game1.m_random.Next(0, 100);

                        if (theNumber < 10)
                        {
                            //its happened too many times! Make an animal!
                            animals.Add(new Animal(1, 100, Prey.preyTextures[Prey.GIRAFFE], 4, 95, 102, 94, 30, Game1.theWyrmPlayer.theWyrm, 1191, 97));
                            numAnimals = numAnimals + 1;
                        }
                        else
                        {
                            //Heck, throw a civvie in there too
                            animals.Add(new Animal(1, 100, Prey.preyTextures[Prey.UNARMEDHUMAN], 4, 24, 21, 23, 6, Game1.theWyrmPlayer.theWyrm, 80, 25));
                            numAnimals = numAnimals + 1;

                            humans.Add(new Animal(1, 100, Prey.preyTextures[Prey.UNARMEDHUMAN], 4, 24, 21, 23, 6, Game1.theWyrmPlayer.theWyrm, 80, 25));
                            numHumans = numAnimals + 1;
                            humans.Add(new Animal(1, 100, Prey.preyTextures[Prey.UNARMEDHUMAN], 4, 24, 21, 23, 6, Game1.theWyrmPlayer.theWyrm, 80, 25));
                            numHumans = numAnimals + 1;
                        }
                    }
                }
                    #endregion
            }
            #endregion

            #region Spawning Humans (non-animals that ran offscreen)

            //Now lets deal with the humans
            HUMAN_SPAWN_ELAPSED = HUMAN_SPAWN_ELAPSED + (float)gametime.ElapsedGameTime.TotalSeconds;

            //is it time to spawn a human?
            if (HUMAN_SPAWN_ELAPSED > HUMAN_SPAWN_DELAY)
            {
                //Yessir!
                //Reset the timer
                HUMAN_SPAWN_ELAPSED = 0;

                //There ARE humans, right?
                if (numHumans > 0)
                {
                    //right
                    Prey p = humans.ElementAt(0);

                    //Add it to the prey list (making it "real")
                    Prey.prey.Add(p);
                    //And remove it from the list of spawnables
                    humans.Remove(p);
                    numHumans = numHumans - 1;
                }
            }

            #endregion

        }

        public static void addAnimal(Prey p)
        {
            //Well, just add it to the list
            animals.Add(p);
            //and increment the count
            numAnimals = numAnimals + 1;
        }

        public static  void addImmediate(Prey p)
        {
            //Well, just add it to the list
            immediates.Add(p);
            //and increment the count
            numImmediates = numImmediates + 1;
        }

        public static  void addHuman(Prey p)
        {
            //Well, just add it to the list
            humans.Add(p);
            //and increment the count
            numHumans = numHumans + 1;
        }


        //When a human prey runs offscreen, it calls this
        //The preyspawner will then add it to the queue of things to spawn later
        //Make sure that the prey is set to walk, i.e. facing the correct direction and in the correct state
        public static void ranOffHuman(Prey p)
        {
            //Well, just add it to the list
            humans.Add(p);
            //and increment the count
            numHumans = numHumans + 1;

            //Then remove it from the prey list, since it just ran off
            removals.Add(p);
            numRemovals = numRemovals + 1;

        }

        //When a nonhuman prey runs offscreen, it calls this
        //The preyspawner will then add it to the queue of things to spawn later
        //Make sure that the prey is set to walk, i.e. facing the correct direction and in the correct state
        public static void ranOffAnimal(Prey p)
        {
            //Well, just add it to the list
            animals.Add(p);
            //and increment the count
            numAnimals = numAnimals + 1;

            //Then remove it from the prey list, since it just ran off
            removals.Add(p);
            numRemovals = numRemovals + 1;
        }

        private bool tooMuchMeat()
        {
            int meatCount = 0;
            foreach (Prey p in Prey.prey)
            {
                meatCount = meatCount + p.meatReward;
            }

            //do we have enought meat?
            if (meatCount > MEAT_SPAWN_LIMIT)
            {
                return true;
            }
            return false;
        }



    }
}
