using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ScoreboardUIController : MonoBehaviour
{
    public ScoreboardElement scoreboardElementPrefab;
    public Transform scoreboardElementsParent;

    public async void Start()
    {
        await LoadScoreboardAsync();
    }

    public async Task LoadScoreboardAsync()
    {
        scoreboardElementsParent.Clear();
        var rpcResult = await SuiApi.Client.GetObjectAsync(Constants.SCOREBOARD_OBJECT_ID);

        if (rpcResult.IsSuccess)
        {
            var scores = JArray.FromObject(rpcResult.Result.Object.Data.Fields["scores"]);
            var scoreboardElements = scores.ToObject<ScoreboardMoveType[]>();

            foreach (var element in scoreboardElements)
            {
                var elementGo = Instantiate(scoreboardElementPrefab, scoreboardElementsParent);
                elementGo.UpdateElementData(element.Fields.Player, element.Fields.Score);
                elementGo.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Could not get scoreboard object with id " + Constants.SCOREBOARD_OBJECT_ID);
        }
    }

    public class ScoreboardMoveType
    {
        public string Type { get; set; }
        public ScoreboardElementFields Fields { get; set; }
    }
    
    public class ScoreboardElementFields
    {
        public string Player { get; set; }
        
        public ulong Score { get; set; }
    }
}
