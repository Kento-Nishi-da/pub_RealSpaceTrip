// ランキング更新
function doPost(e) {
    // 受け取ったJSONデータを取得

    try {
        var jsonString = e.postData.getDataAsString();
        var res = JSON.stringify({ "response": "OK" });
    } catch (e) {
        // エラーが発生した場合はエラーメッセージを返す
        return {
            "response": "NG" + e.toString()
        };
    }

    // JSONデータをパースしてオブジェクトに変換
    var jsonData = JSON.parse(jsonString);
    var data = jsonData.datas;

    // スプレッドシートのシートを取得
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('score');

    // 書き換える範囲の開始セルを指定
    var startRow = 2;
    var startColumn = 1;

    // JSONデータに対応するセルの値を書き換え
    for (var i = 0; i < data.length; i++) {
        var rowData = data[i];
        var name = rowData.name;
        var score = rowData.score;

        // 範囲指定して書き込み
        sheet.getRange(startRow + i, startColumn).setValue(name);
        sheet.getRange(startRow + i, startColumn + 1).setValue(score);
    }


    // 返却用データ設定
    let output = ContentService.createTextOutput();
    output.setMimeType(ContentService.MimeType.JSON);
    output.setContent(res);
    console.log(res);

    return output;
}
