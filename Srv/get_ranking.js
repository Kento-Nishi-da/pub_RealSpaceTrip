// ランキング取得
function doGet(e) {
    // スプレッドシートのシートを取得
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('score');
  
    // 取得したいデータの範囲を指定
    var range = sheet.getRange('A2:B11');
  
    // 範囲から値を取得
    var values = range.getValues();
  
    // JSONデータの配列を初期化
    var jsonData = [];
  
    // データをJSON形式に変換
    for (var i = 0; i < values.length; i++) {
      var row = {
        "name": values[i][0],   // 列Aの値を"name"として追加
        "score": values[i][1]   // 列Bの値を"score"として追加
      };
      //console.log (values[i][0]);
      //console.log (values[i][1]);
      jsonData.push(row);
    }
  
    console.log(jsonData);
  
    let res = JSON.stringify({"datas": jsonData});
    //console.log(ret);
  
    // 返却用データ設定
    let output = ContentService.createTextOutput();
    output.setMimeType(ContentService.MimeType.JSON);
    output.setContent(res);
    console.log(res);
    return output;
  
  }
  