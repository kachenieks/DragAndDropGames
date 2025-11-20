# DragAndDropGames
Unity 2D Drag And Drop games for children +6

**To do list:**
- [x] Project folder structure created  
- [x] All required assets imported  
- [x] CityGame scene objects placed  
- [x] Drag & Drop script implemented  
- [x] Object transformation script  
- [x] Object snapping & win-logic implemented  
- [x] Sounds and audio effects added  
- [x] Timer and star-rating system  
- [x] Camera zoom in/out  
- [x] Animated main menu  
- [x] Interstitial Ads between scenes  
- [x] Banner Ads in CityGame + HanojasTornis  
- [x] Rewarded Ads in CityGame (adds extra time)  
- [x] Rewarded Ads in HanojasTornis (removes 1 move, never below 0)  
- [x] Fully working Hanoi Tower with 6 blocks  
- [x] Physics added: blocks fall naturally and align correctly  
- [x] Only the top block can be picked up  
- [x] Win condition when all blocks are moved to the chosen tower  
- [x] Win panel + animations  
- [x] Reset function added  
- [x] Android touch support  
<<<<<<< HEAD

=======
>>>>>>> a24b8833278d59506d46632a08e7c463886d097c


**🕹️ Game Overview:**
This project includes multiple Unity scenes connected together to create a small drag-and-drop style game.

**In the main menu, you can choose to:**
1. Play City Game
2. Quit
3. Go to Game 2 - is not active currently
<img width="1919" height="1079" alt="Ekrānuzņēmums 2025-10-13 124159" src="https://github.com/user-attachments/assets/87fe29a7-6882-42d6-b500-1c82c5002fce" />

In City Game, you can play a drag and drop style mini-game.
From this scene, you can also return to the main menu.

During the game, a timer is displayed.
When the game is completed, a win screen appears showing:

**Your completion time**
**Stars based on your performance:**
⏱️ under 60 seconds → ⭐⭐⭐
⏱️ under 90 seconds → ⭐⭐
⏱️ more than 90 seconds → ⭐
<img width="1919" height="1079" alt="Ekrānuzņēmums 2025-10-13 124219" src="https://github.com/user-attachments/assets/e1a68888-01e2-44e1-979b-ef2e7405d9c5" />

From the **win screen**, you can choose to:
1. Restart the game
2. Go back to the main menu
<img width="1919" height="1079" alt="Ekrānuzņēmums 2025-10-13 124343" src="https://github.com/user-attachments/assets/80da4c04-fec6-49d5-b65b-d781e8ed66f8" />


**Reset screen** you can choose to:
1. Restart the game
2. Go back to the main menu
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/2990ddfc-e13c-4434-805e-9d669366e706" />

**Hanoi Tower Mini-Game**
In this mini-game, your goal is to move all 6 blocks from the first pole to the final pole while respecting classic Tower of Hanoi rules:
Only one block can be moved at a time
You cannot place a larger block on a smaller one
Only the top block of a pole can be picked up
<img width="759" height="405" alt="image" src="https://github.com/user-attachments/assets/2e0e90fb-1d1c-4da9-9fb7-f47253939089" />
Example screenshot: Gameplay view of the Hanoi Tower puzzle.
The game includes a smooth drag-and-drop system, stable block stacking, and perfect positioning logic.

When all blocks are moved to the target pole in the correct order, the victory screen appears.
**The victory screen shows:**

✔ Total number of moves
✔ The optimal number of moves (for 6 blocks: 63)
✔ A congratulatory message
✔ Buttons to restart the puzzle or return to the main menu

<img width="806" height="402" alt="image" src="https://github.com/user-attachments/assets/6d428941-749b-453e-a441-ebd1215e7e0e" />
Example screenshot: Victory panel after completing the Hanoi Tower challenge.

🎬 Rewarded Ads – Three Different Bonus Types
<img width="155" height="50" alt="image" src="https://github.com/user-attachments/assets/e681cfca-ed23-4193-9ba9-00f2c067fd46" />
This project includes three different types of rewarded ads, one for each game mode and scenario.
Each ad type gives a different bonus depending on the scene where it is watched.

Here are the reward types:

**🟨 1. City Game – Extra Time**
If the player watches a rewarded ad in the City Game scene, they receive:
⏱️ +10 seconds of bonus time
This helps the player finish the level faster and earn more stars.
Example screenshot: Rewarded Ad button inside City Game.

**🟥 2. City Game – Clear All Flying Objects**
Another rewarded ad option in City Game allows the player to instantly remove all obstacles:
🧹 Destroys every flying object on the screen
This makes it easier to finish the level without distractions.
Example screenshot: Flying object removal rewarded ad.

**🟩 3. Hanoi Tower – Remove 1 Move**
In the Hanoi Tower (HanojasTornis) scene, watching a rewarded ad gives:
➖ Removes 1 move from the total count
✔ Never goes below 0
✔ Helps players get closer to the perfect score (63 moves)
Example screenshot: Rewarded ad available in the Hanoi Tower puzzle.

🎯 Summary of Rewards
Scene	Reward Type	Effect
CityGame	Extra Time	+10 seconds
CityGame	Clear Screen	Removes all flying objects
Hanoi Tower	Move Reduction	-1 move (never below 0)
