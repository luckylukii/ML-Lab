using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class EnemyCar : MonoBehaviour {
    [SerializeField] private Sprite[] skins;
    public System.Action OnCollided;
    public async void Init(GameObject player, float yPos, float speed) {
        GetComponent<Image>().sprite = skins[Random.Range(0, skins.Length)];
        
        RectTransform car = GetComponent<RectTransform>();
        car.anchoredPosition = new Vector2(Screen.width / 2, yPos);
        while (car.position.x > -Screen.width / 2) {
            car.position = new Vector3(Mathf.MoveTowards(car.position.x, -Screen.width, speed * Time.deltaTime), yPos);
            
            Vector2 playerPos = player.transform.position;
            if (Mathf.Abs(playerPos.x - car.position.x) <= 50 && playerPos.y == car.position.y) {
                Debug.Log("Lost");
                break;
            }


            await Task.Yield();
        }

        OnCollided?.Invoke();
    }
}