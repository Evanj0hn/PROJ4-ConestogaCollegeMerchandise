const API_BASE_URL = "https://localhost:7235/api/Merchandise";

document.addEventListener("DOMContentLoaded", () => {
    // 1. GET: Display items
    const merchandiseList = document.getElementById("merchandise-list");
    const refreshBtn = document.getElementById("refresh-btn");

    refreshBtn.addEventListener("click", getMerchandise);
    // Load items on page load
    getMerchandise();

    function getMerchandise() {
        fetch(API_BASE_URL)
            .then(response => response.json())
            .then(data => {
                merchandiseList.innerHTML = ""; // Clear the current list
                if (data.length === 0) {
                    merchandiseList.innerHTML = "<p>No products available.</p>";
                } else {
                    data.forEach(item => {
                        const card = document.createElement("div");
                        card.className = "card";
                        card.innerHTML = `
              <img src="${item.imageUrl || 'https://via.placeholder.com/150'}" alt="${item.name}" />
              <h3>${item.name}</h3>
              <p>${item.description}</p>
              <p>Price: $${item.price.toFixed(2)}</p>
              <p>ID: ${item.id}</p>
            `;
                        merchandiseList.appendChild(card);
                    });
                }
            })
            .catch(err => console.error("GET Error:", err));
    }

    // 2. POST: Add a new item
    const addForm = document.getElementById("add-product-form");
    addForm.addEventListener("submit", e => {
        e.preventDefault();
        const newItem = {
            name: document.getElementById("post-name").value,
            description: document.getElementById("post-description").value,
            price: parseFloat(document.getElementById("post-price").value),
            imageUrl: document.getElementById("post-imageUrl").value
        };

        fetch(API_BASE_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newItem)
        })
            .then(res => {
                if (!res.ok) throw new Error("Failed to POST item.");
                return res.json();
            })
            .then(createdItem => {
                console.log("Item created:", createdItem);
                addForm.reset();
                getMerchandise(); // refresh the list
            })
            .catch(err => console.error("POST Error:", err));
    });

    // 3. PUT: Update an entire item
    const putForm = document.getElementById("put-product-form");
    putForm.addEventListener("submit", e => {
        e.preventDefault();
        const id = parseInt(document.getElementById("put-id").value, 10);
        const updatedItem = {
            id: id,
            name: document.getElementById("put-name").value,
            description: document.getElementById("put-description").value,
            price: parseFloat(document.getElementById("put-price").value),
            imageUrl: document.getElementById("put-imageUrl").value
        };

        fetch(`${API_BASE_URL}/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updatedItem)
        })
            .then(res => {
                if (!res.ok) throw new Error("Failed to PUT item.");
                console.log("Item updated.");
                putForm.reset();
                getMerchandise();
            })
            .catch(err => console.error("PUT Error:", err));
    });

    // 4. PATCH: Partially update the price
    const patchForm = document.getElementById("patch-product-form");
    patchForm.addEventListener("submit", e => {
        e.preventDefault();
        const id = parseInt(document.getElementById("patch-id").value, 10);
        const newPrice = parseFloat(document.getElementById("patch-price").value);

        // JSON Patch format
        const patchDoc = [
            { op: "replace", path: "/price", value: newPrice }
        ];

        fetch(`${API_BASE_URL}/${id}`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json-patch+json" },
            body: JSON.stringify(patchDoc)
        })
            .then(res => {
                if (!res.ok) throw new Error("Failed to PATCH item.");
                console.log("Item patched.");
                patchForm.reset();
                getMerchandise();
            })
            .catch(err => console.error("PATCH Error:", err));
    });

    // 5. DELETE: Remove an item
    const deleteForm = document.getElementById("delete-product-form");
    deleteForm.addEventListener("submit", e => {
        e.preventDefault();
        const id = parseInt(document.getElementById("delete-id").value, 10);

        fetch(`${API_BASE_URL}/${id}`, {
            method: "DELETE"
        })
            .then(res => {
                if (!res.ok) throw new Error("Failed to DELETE item.");
                console.log("Item deleted.");
                deleteForm.reset();
                getMerchandise();
            })
            .catch(err => console.error("DELETE Error:", err));
    });

    // 6. OPTIONS: Check supported methods
    const optionsBtn = document.getElementById("options-btn");
    const optionsResult = document.getElementById("options-result");

    optionsBtn.addEventListener("click", () => {
        fetch(API_BASE_URL, { method: "OPTIONS" })
            .then(res => {
                const allowHeader = res.headers.get("Allow");
                if (allowHeader) {
                    optionsResult.innerText = `Allowed methods: ${allowHeader}`;
                } else {
                    optionsResult.innerText = "No Allow header returned.";
                }
            })
            .catch(err => console.error("OPTIONS Error:", err));
    });
});
