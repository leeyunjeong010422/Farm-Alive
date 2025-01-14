using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SectionInfo : UIBinder
{
    private string _weatherString = "Text_Weather";
    private string _blightString = "Text_Blight";
    private string _temperatureString = "Text_Temperature";

    private TextMeshProUGUI _weatherText;
    private TextMeshProUGUI _blightText;
    private TextMeshProUGUI _temperatureText;

    private void Start()
    {
        _weatherText = GetUI<TextMeshProUGUI>(_weatherString);
        _blightText = GetUI<TextMeshProUGUI>(_blightString);
        _temperatureText = GetUI<TextMeshProUGUI>(_temperatureString);

        EventManager.Instance.OnEventStarted.AddListener(OnWeatherRaised);
        EventManager.Instance.OnEventStarted.AddListener(OnBlightRaised);
        EventManager.Instance.OnEventStarted.AddListener(OnTemperatureRaised);

        EventManager.Instance.OnEventEnded.AddListener(OnWheatherStopped);
        EventManager.Instance.OnEventEnded.AddListener(OnBlightStopped);
        EventManager.Instance.OnEventEnded.AddListener(OnTemperatureStopped);
    }

    private void OnWeatherRaised(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "ÆøÇ³":
            case "Æø¿ì":
            case "°¡¹³":
                UpdateWeatherText();
                break;
            default:
                break;
        }
    }

    private void OnWheatherStopped(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "ÆøÇ³":
            case "Æø¿ì":
            case "°¡¹³":
                UpdateWeatherText();
                break;
            default:
                break;
        }
    }

    private void UpdateWeatherText()
    {
        _weatherText.text = "³¯¾¾: ";
        int activeWeather = 0;

        foreach (int activeID in EventManager.Instance._activeEventsID)
        {
            string eventName = CSVManager.Instance.Events[activeID].event_name;
            switch (eventName)
            {
                case "ÆøÇ³":
                case "Æø¿ì":
                case "°¡¹³":
                    _weatherText.text += $"{CSVManager.Instance.Events[activeID].event_name} ";
                    activeWeather++;
                    break;
                default:
                    break;
            }
        }

        if (activeWeather == 0)
            _weatherText.text = "³¯¾¾: Á¤»ó";
    }

    private void OnBlightRaised(GameData.EVENT eventData)
    {
        if (eventData.event_name != "º´ÃæÇØ")
            return;

        _blightText.text = $"ÇØÃæ: ¹ß»ý";
    }

    private void OnBlightStopped(GameData.EVENT eventData)
    {
        if (eventData.event_name != "º´ÃæÇØ")
            return;

        _blightText.text = $"ÇØÃæ: Á¤»ó";
    }

    private void OnTemperatureRaised(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "¿Âµµ »ó½Â":
                _temperatureText.text = $"±â¿Â: ³ôÀ½";
                break;
            case "¿Âµµ ÇÏ°­":
                _temperatureText.text = $"±â¿Â: ³·À½";
                break;
            default:
                break;
        }
    }

    private void OnTemperatureStopped(GameData.EVENT eventData)
    {
        switch (eventData.event_name)
        {
            case "¿Âµµ »ó½Â":
            case "¿Âµµ ÇÏ°­":
                _temperatureText.text = $"±â¿Â: Á¤»ó";
                break;
            default:
                break;
        }
    }
}
