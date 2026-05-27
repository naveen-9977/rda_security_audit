<%@ Page Language="C#" %>
<!DOCTYPE html>
<html>
<head><title>ASPX Test</title></head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Execution Success!</h2>
            <p>The current server time is: <%= DateTime.Now.ToString() %></p>
            <p>Math test (2 + 2): <%= 2 + 2 %></p>
        </div>
    </form>
</body>
</html>
