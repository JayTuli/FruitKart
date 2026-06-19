import { CATEGORY, SPECIAL_TAG } from "../../utility/constants";
import { toast } from "react-toastify";

function MenuItemModal({
  onClose,
  isSubmitting,
  formData,
  onSubmit,
  onChange,
  isEditing,
}) {
  const handleSubmit = (e) => {
    e.preventDefault();
    const errors = [];

    if (!formData.name?.trim()) {
      errors.push("Name is required");
    }

    if (!formData.category?.trim()) {
      errors.push("Category is required");
    }

    if (
      !formData.price ||
      parseFloat(formData.price) <= 0 ||
      parseFloat(formData.price) >= 1000
    ) {
      errors.push("Price is required and must be between 1 and 1000");
    }

    if (
      formData.stockCount === "" ||
      formData.stockCount === undefined ||
      formData.stockCount === null ||
      parseInt(formData.stockCount) < 0
    ) {
      errors.push("Stock count is required and must be 0 or more");
    }

    if (errors.length > 0) {
      toast.error(
        <div>
          <strong>Please correct the following:</strong>
          <ul className="mb-0 mt-1 ps-3">
            {errors.map((error, index) => (
              <li key={index}>{error}</li>
            ))}
          </ul>
        </div>
      );
      return;
    }

    onSubmit(formData);
  };

  return (
    <>
      <div className="modal-backdrop fade show" />

      <div
        className="modal fade show"
        style={{ display: "block" }}
        tabIndex="-1"
        role="dialog"
      >
        <div className="modal-dialog modal-lg" role="document">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">
                {isEditing ? "Edit Menu Item" : "Add New Menu Item"}
              </h5>
              <button
                type="button"
                className="btn-close"
                aria-label="Close"
                onClick={onClose}
              />
            </div>

            <div className="modal-body">
              <form onSubmit={handleSubmit}>

                {/* Row 1: Name + Category */}
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Name *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="name"
                        value={formData.name || ""}
                        onChange={onChange}
                      />
                    </div>
                  </div>
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Category *</label>
                      <select
                        className="form-select"
                        name="category"
                        value={formData.category || ""}
                        onChange={onChange}
                      >
                        <option value="">Select Category</option>
                        {CATEGORY.map((category) => (
                          <option value={category} key={category}>
                            {category}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>

                {/* Description */}
                <div className="mb-3">
                  <label className="form-label">Description</label>
                  <textarea
                    className="form-control"
                    name="description"
                    rows="3"
                    value={formData.description || ""}
                    onChange={onChange}
                  />
                </div>

                {/* Row 2: Price + Special Tag */}
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Price * (₹)</label>
                      <input
                        type="number"
                        className="form-control"
                        name="price"
                        step="0.01"
                        min="0.01"
                        value={formData.price || ""}
                        onChange={onChange}
                      />
                    </div>
                  </div>
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Special Tag</label>
                      <select
                        className="form-select"
                        name="specialTag"
                        value={formData.specialTag || ""}
                        onChange={onChange}
                      >
                        {SPECIAL_TAG.map((tag) => (
                          <option value={tag} key={tag}>
                            {tag}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>

                {/* Row 3: Stock Count */}
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Stock Count *</label>
                      <input
                        type="number"
                        className="form-control"
                        name="stockCount"
                        min="0"
                        max="10000"
                        value={formData.stockCount ?? ""}
                        onChange={onChange}
                      />
                      <div className="form-text">
                        Set to 0 to mark item as out of stock
                      </div>
                    </div>
                  </div>
                </div>

                {/* Image Upload */}
                <div className="mb-4">
                  <label className="form-label">Image</label>
                  <input
                    type="file"
                    className="form-control"
                    name="image"
                    onChange={onChange}
                    accept="image/*"
                  />
                  <div className="form-text">
                    Upload an image for the menu item
                  </div>
                </div>

                {/* Actions */}
                <div className="d-flex justify-content-end gap-2">
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={onClose}
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={isSubmitting}
                  >
                    {isSubmitting ? (
                      <span className="spinner-border spinner-border-sm me-2" />
                    ) : (
                      <>{isEditing ? "Update Menu Item" : "Create Menu Item"}</>
                    )}
                  </button>
                </div>

              </form>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default MenuItemModal;
