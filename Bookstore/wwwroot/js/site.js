const API_BASE = "http://localhost:50536/api/bookinfo"; 

async function searchBooks() {
    const input = document.getElementById("searchInput");
    const resultDiv = document.getElementById("results");

    const query = input.value.trim();

    resultDiv.innerHTML = "";

    if (!query) {
        resultDiv.innerHTML = "<p>Please enter a value.</p>";
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/searchbookwithquery?query=${encodeURIComponent(query)}`);

        if (response.status === 404) {
            showError("No books found.");
            return;
        }

        if (!response.ok) {
            throw new Error(`Error: ${response.statusText}`);
            return;
        }

        const data = await response.json();

        renderBooks(data);
    }
    catch(error) {
        showError("Something went wrong.");
        console.error(error);
    }
}

function renderBooks(books) {
    const resultDiv = document.getElementById("results");

    if (!books || books.length === 0) {
        showError("No books found.");
        return;
    }

    books.forEach(book => {
        const card = `
            <div class="card mb-3">
                <div class="card-body">
                    <h5 class="card-title">${book.Title}</h5>
                    <p class="card-author">${book.Author}</p>
                    <p class="card-text">${book.ISBN}</p>
                </div>
            </div>
        `;

        resultDiv.innerHTML += card;
    });
}


function showError(message) {
    const resultDiv = document.getElementById("results");

    resultDiv.innerHTML = `
        <div class="alert alert-danger">
            ${message}
        </div>
    `;
}

async function createBook() {
    const author = document.getElementById("authorInput").value.trim();
    const title = document.getElementById("titleInput").value.trim();
    const isbn = document.getElementById("isbnInput").value.trim();

    const resultDiv = document.getElementById("createResult");

    resultDiv.innerHTML = "";

    if (!author || !title || !isbn) {
        resultDiv.innerHTML = "<div class='alert alert-danger'>All fields are required.</div>";
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/createbook`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Author: author,
                Title: title,
                ISBN: isbn
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            resultDiv.innerHTML = `<div class='alert alert-danger'>${errorText}</div>`;
            return;
        }

        resultDiv.innerHTML = "<div class='alert alert-success'>Book added successfully!</div>";

        document.getElementById("authorInput").value = "";
        document.getElementById("titleInput").value = "";
        document.getElementById("isbnInput").value = "";

    } catch (error) {
        console.error(error);
        resultDiv.innerHTML = "<div class='alert alert-danger'>Something went wrong.</div>";
    }
}

async function suggestBook() {
    const name = document.getElementById("nameSuggestion").value.trim();
    const author = document.getElementById("authorSuggestion").value.trim();
    const title = document.getElementById("titleSuggestion").value.trim();

    const resultDiv = document.getElementById("suggestionResult");

    if (!name || !author || !title) {
        resultDiv.innerHTML = `<div class='alert alert-danger'>All fields required</div>`;
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/suggestbook`, {
            method: "POST", 
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Name: name,
                Author: author,
                Title: title,
            })
        });

        if (!response.ok) {
            const errorText = await response.text()
            resultDiv.innerHTML = `<div class='alert alert-danger'>${errorText}</div>`;
            return
        }

        resultDiv.innerHTML = "<div class='alert alert-success'>Book added successfully!</div>";

        document.getElementById("nameSuggestion").value = "";
        document.getElementById("authorSuggestion").value = ""; 
        document.getElementById("titleSuggestion").value = "";

    } catch (error) {
        console.error(error);
        resultDiv.innerHTML = "<div class='alert alert-danger'>Something went wrong.</div>";
    }
} 