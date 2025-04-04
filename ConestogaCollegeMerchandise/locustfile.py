from locust import HttpUser, task, between
import random
import urllib3

# Disable SSL warnings
urllib3.disable_warnings()

class AdminBehavior(HttpUser):
    wait_time = between(1, 3)

    def on_start(self):
        self.client.verify = False  # Add this line

        self.new_item = {
            "name": "Locust Hoodie",
            "description": "Load test hoodie",
            "price": 34.99,
            "imageUrl": "https://via.placeholder.com/150"
        }
        response = self.client.post("/api/Merchandise", json=self.new_item)
        if response.status_code == 201:
            self.item_id = response.json()["id"]
        else:
            self.item_id = None

    @task
    def get_merchandise(self):
        self.client.get("/api/Merchandise")

    @task
    def patch_price(self):
        if self.item_id:
            patch_doc = [
                { "op": "replace", "path": "/price", "value": round(random.uniform(10, 50), 2) }
            ]
            self.client.patch(f"/api/Merchandise/{self.item_id}",
                              json=patch_doc,
                              headers={"Content-Type": "application/json-patch+json"})

    @task
    def delete_merchandise(self):
        if self.item_id:
            self.client.delete(f"/api/Merchandise/{self.item_id}")
            self.item_id = None
