// 翻訳
function doGet(e) {
    // GETリクエストからパラメータを取得
    var title = e.parameter.title;
    var explanation = e.parameter.explanation;
    var target_language = e.parameter.target_language;
  
      // JSONデータの配列を初期化
    var jsonData = [];
  
    // パラメータが正しく指定されているか確認
    if (!title || !explanation || !target_language) {
      jsonData = { 
        "error": "パラメータに誤りがあります" 
        };
      return ContentService.createTextOutput(
        JSON.stringify(jsonData)).setMimeType(ContentService.MimeType.JSON);
    }
  
    // 文を翻訳してJSON形式で結果を返す
    var translatedText = TranslateText(title, explanation, target_language);
    jsonData = translatedText;
    console.log(jsonData);
    return ContentService.createTextOutput(JSON.stringify(translatedText)).setMimeType(ContentService.MimeType.JSON);
  }
  
  // 翻訳して翻訳後の分を返す
  function TranslateText(_title, _explanation, _language) {
    try {
      // LanguageAppクラスを使用して文を翻訳
      // 第2引数は翻訳元の言語だが、空文字で自動判別できる
      var translatedTitle = LanguageApp.translate(_title, "", _language);
      var translatedEx = LanguageApp.translate(_explanation, "", _language);
      return {
        "response": "OK",
        "translated_title": translatedTitle,
        "translated_ex": translatedEx
      };
    } catch (e) {
      // エラーが発生した場合はエラーメッセージを返す
      return {
        "response": "NG" + e.toString()
      };
    }
  }
  