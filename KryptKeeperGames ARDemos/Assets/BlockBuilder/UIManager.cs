using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KryptKeeperGamesARDemo.BlockBuilder
{
    public class UIManager : MonoBehaviour
    {
        public delegate void DrawModeButtonPressed();
        public static DrawModeButtonPressed onDrawModeButtonPressed;

        public delegate void BlockDisplayButtonPressed(int blockIndex);
        public static BlockDisplayButtonPressed onBlockDisplayButtonPressed;

        public delegate void EnemyDisplayButtonPressed(int enemyIndex);
        public static EnemyDisplayButtonPressed onEnemyDisplayButtonPressed;

        public Text drawModeText;
        bool drawModeOn = true;

        public GameObject blockDisplayPanel;
        public GameObject enemyDisplayPanel;

        public Image blockImage;
        public Image enemyImage;

        public Sprite[] blockImages;
        public Sprite[] enemyImages;

        public void ToggleDrawMode()
        {
            //drawModeOn = !drawModeOn;
            //drawModeText.text = drawModeOn ? "Draw" : "Erase";

            if(onDrawModeButtonPressed != null)
            {
                onDrawModeButtonPressed();
            }
            if (BlockBuilderMode.instance.buildState == BlockBuilderMode.eBuildState.REMOVE) drawModeText.text = "ERASE";
            else drawModeText.text = "DRAW";
        }

        public void ToggleBlockButtons()
        {
            if (blockDisplayPanel.activeSelf)
                blockDisplayPanel.SetActive(false);
            else blockDisplayPanel.SetActive(true);
        }

        public void ChangeBlock(int blockID)
        {
            if(onBlockDisplayButtonPressed != null)
            {
                onBlockDisplayButtonPressed(blockID);
                blockImage.sprite = blockImages[blockID];
                ToggleBlockButtons();
            }
        }

        public void ToggleEnemyButtons()
        {
            if (enemyDisplayPanel.activeSelf)
                enemyDisplayPanel.SetActive(false);
            else enemyDisplayPanel.SetActive(true);
        }

        public void ChangeEnemy(int enemyID)
        {
            if (onEnemyDisplayButtonPressed != null)
            {
                onEnemyDisplayButtonPressed(enemyID);
                enemyImage.sprite = enemyImages[enemyID];
                ToggleEnemyButtons();
            }
        }
    }
}
