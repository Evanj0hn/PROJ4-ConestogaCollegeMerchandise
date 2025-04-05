import requests

BASE_URL = "https://localhost:7235/api/Merchandise"
VERIFY_SSL = False  # Disable SSL verification for local self-signed certs

def test_get_all_merchandise():
    response = requests.get(BASE_URL, verify=VERIFY_SSL)
    assert response.status_code == 200
    assert isinstance(response.json(), list)

def test_post_valid_merchandise():
    item = {
        "name": "pytest hoodie",
        "description": "functional test",
        "price": 29.99,
        "imageUrl": "https://via.placeholder.com/150"
    }
    response = requests.post(BASE_URL, json=item, verify=VERIFY_SSL)
    assert response.status_code == 201
    data = response.json()
    assert data["name"] == item["name"]
    return data["id"]

def test_post_invalid_merchandise():
    item = {
        "description": "missing name and price",
        "imageUrl": "https://via.placeholder.com/150"
    }
    response = requests.post(BASE_URL, json=item, verify=VERIFY_SSL)
    assert response.status_code == 400 or response.status_code == 422

def test_patch_valid_price():
    item_id = test_post_valid_merchandise()
    patch = [{"op": "replace", "path": "/price", "value": 15.99}]
    response = requests.patch(
        f"{BASE_URL}/{item_id}",
        json=patch,
        headers={"Content-Type": "application/json-patch+json"},
        verify=VERIFY_SSL
    )
    assert response.status_code == 204

def test_patch_invalid_field():
    item_id = test_post_valid_merchandise()
    patch = [{"op": "replace", "path": "/notARealField", "value": "test"}]
    response = requests.patch(
        f"{BASE_URL}/{item_id}",
        json=patch,
        headers={"Content-Type": "application/json-patch+json"},
        verify=VERIFY_SSL
    )
    assert response.status_code == 400 or response.status_code == 422

def test_delete_existing_merchandise():
    item_id = test_post_valid_merchandise()
    response = requests.delete(f"{BASE_URL}/{item_id}", verify=VERIFY_SSL)
    assert response.status_code == 204

def test_delete_twice():
    item_id = test_post_valid_merchandise()
    response = requests.delete(f"{BASE_URL}/{item_id}", verify=VERIFY_SSL)
    assert response.status_code == 204
    response = requests.delete(f"{BASE_URL}/{item_id}", verify=VERIFY_SSL)
    assert response.status_code == 404
