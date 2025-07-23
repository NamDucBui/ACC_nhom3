import React, { useState, useEffect } from "react";

const API_URL = "http://localhost:5259/api/tourproducts";

const TOUR_SITES = [
  { value: "traveloka", label: "Traveloka" },
  { value: "vietravel", label: "Vietravel" },
  { value: "dulichviet", label: "Du Lịch Việt" },
];

function App() {
  const [site, setSite] = useState(TOUR_SITES[0].value);
  const [crawlTime, setCrawlTime] = useState("now");
  const [pages, setPages] = useState(1);
  const [tours, setTours] = useState([]);
  const [loading, setLoading] = useState(false);
  const [editTour, setEditTour] = useState(null);
  const [form, setForm] = useState({
    title: "",
    price: "",
    startDate: "",
    location: "",
    duration: "",
    imageUrl: "",
    detailUrl: "",
  });

  useEffect(() => {
    fetchTours();
  }, []);

  const fetchTours = async () => {
    setLoading(true);
    try {
      const res = await fetch(API_URL);
      const data = await res.json();
      setTours(data);
    } catch {
      alert("Không lấy được dữ liệu từ backend");
    }
    setLoading(false);
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Bạn chắc chắn muốn xóa?")) return;
    await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    fetchTours();
  };

  const handleEdit = (tour) => {
    setEditTour(tour.id);
    setForm({
      title: tour.title,
      price: tour.price,
      startDate: tour.startDate ? tour.startDate.slice(0, 10) : "",
      location: tour.location || "",
      duration: tour.duration || "",
      imageUrl: tour.imageUrl || "",
      detailUrl: tour.detailUrl || "",
    });
  };

  const handleFormChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();
    const payload = {
      ...form,
      price: form.price ? parseFloat(form.price) : null,
      startDate: form.startDate || null,
      id: editTour,
      sourceId: 1,
      detailUrl: form.detailUrl,
      lastUpdatedAt: new Date().toISOString(),
    };

    const method = editTour ? "PUT" : "POST";
    const url = editTour ? `${API_URL}/${editTour}` : API_URL;

    await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    setEditTour(null);
    setForm({ title: "", price: "", startDate: "", location: "", duration: "", imageUrl: "", detailUrl: "" });
    fetchTours();
  };

  const handleCancelEdit = () => {
    setEditTour(null);
    setForm({ title: "", price: "", startDate: "", location: "", duration: "", imageUrl: "", detailUrl: "" });
  };

  const handleCrawl = async () => {
    setLoading(true);

    let crawlUrl = "";
    if (site === "traveloka") {
      crawlUrl = "https://www.traveloka.com/vi-vn/hotel";
    } else {
      alert("Hiện tại chỉ hỗ trợ crawl Traveloka.");
      setLoading(false);
      return;
    }

    try {
      const res = await fetch("http://localhost:5259/api/crawl/traveloka-hotel", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ url: crawlUrl }),
      });

      if (res.ok) {
        alert("✅ Đã crawl xong dữ liệu!");
        fetchTours();
      } else {
        alert("❌ Lỗi khi crawl dữ liệu.");
      }
    } catch (error) {
      console.error(error)
      alert("❌ Không thể gọi API crawl.");
    }

    setLoading(false);
  };

  return (
    <div style={{ maxWidth: 1000, margin: "auto", padding: 24 }}>
      <h1>Crawl Tour Du Lịch</h1>

      <div style={{ marginBottom: 16 }}>
        <label>Chọn trang web: </label>
        <select value={site} onChange={e => setSite(e.target.value)}>
          {TOUR_SITES.map(s => (
            <option key={s.value} value={s.value}>{s.label}</option>
          ))}
        </select>
      </div>

      <div style={{ marginBottom: 16 }}>
        <label>Thời gian crawl: </label>
        <select value={crawlTime} onChange={e => setCrawlTime(e.target.value)}>
          <option value="now">Ngay bây giờ</option>
          <option value="schedule">Lên lịch</option>
        </select>
        {crawlTime === "schedule" && (
          <input type="datetime-local" style={{ marginLeft: 8 }} />
        )}
      </div>

      <div style={{ marginBottom: 16 }}>
        <label>Số trang muốn crawl: </label>
        <input
          type="number"
          min={1}
          value={pages}
          onChange={e => setPages(e.target.value)}
          style={{ width: 60, marginLeft: 8 }}
        />
      </div>

      <button onClick={handleCrawl} disabled={loading}>
        {loading ? "Đang crawl..." : "Bắt đầu crawl"}
      </button>

      <h2 style={{ marginTop: 32 }}>Danh sách tour đã crawl</h2>
      {loading && <div>Đang tải...</div>}

      <table border="1" cellPadding={8} style={{ width: "100%", marginTop: 16 }}>
        <thead>
          <tr>
            <th>Tên tour</th>
            <th>Giá</th>
            <th>Ngày khởi hành</th>
            <th>Địa điểm</th>
            <th>Thời lượng</th>
            <th>Ảnh</th>
            <th>Chi tiết</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {tours.length === 0 ? (
            <tr>
              <td colSpan={8} style={{ textAlign: "center" }}>Chưa có dữ liệu</td>
            </tr>
          ) : (
            tours.map((tour) => (
              editTour === tour.id ? (
                <tr key={tour.id} style={{ background: "#ffe" }}>
                  <td colSpan={8}>
                    <form onSubmit={handleFormSubmit} style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
                      <input name="title" value={form.title} onChange={handleFormChange} placeholder="Tên tour" required />
                      <input name="price" value={form.price} onChange={handleFormChange} placeholder="Giá" type="number" min={0} />
                      <input name="startDate" value={form.startDate} onChange={handleFormChange} type="date" />
                      <input name="location" value={form.location} onChange={handleFormChange} placeholder="Địa điểm" />
                      <input name="duration" value={form.duration} onChange={handleFormChange} placeholder="Thời lượng" />
                      <input name="imageUrl" value={form.imageUrl} onChange={handleFormChange} placeholder="Ảnh" />
                      <input name="detailUrl" value={form.detailUrl} onChange={handleFormChange} placeholder="Link chi tiết" />
                      <button type="submit">Lưu</button>
                      <button type="button" onClick={handleCancelEdit}>Hủy</button>
                    </form>
                  </td>
                </tr>
              ) : (
                <tr key={tour.id}>
                  <td>{tour.title}</td>
                  <td>{tour.price}</td>
                  <td>{tour.startDate ? tour.startDate.slice(0, 10) : ""}</td>
                  <td>{tour.location}</td>
                  <td>{tour.duration}</td>
                  <td>
                    {tour.imageUrl && <img src={tour.imageUrl} alt="tour" style={{ width: 60 }} />}
                  </td>
                  <td>
                    {tour.detailUrl && <a href={tour.detailUrl} target="_blank" rel="noreferrer">Xem</a>}
                  </td>
                  <td>
                    <button onClick={() => handleEdit(tour)}>Sửa</button>
                    <button onClick={() => handleDelete(tour.id)}>Xóa</button>
                  </td>
                </tr>
              )
            ))
          )}
        </tbody>
      </table>

      <h3 style={{ marginTop: 32 }}>Thêm tour mới</h3>
      <form onSubmit={handleFormSubmit} style={{ display: "flex", gap: 8, flexWrap: "wrap", marginBottom: 32 }}>
        <input name="title" value={form.title} onChange={handleFormChange} placeholder="Tên tour" required />
        <input name="price" value={form.price} onChange={handleFormChange} placeholder="Giá" type="number" min={0} />
        <input name="startDate" value={form.startDate} onChange={handleFormChange} type="date" />
        <input name="location" value={form.location} onChange={handleFormChange} placeholder="Địa điểm" />
        <input name="duration" value={form.duration} onChange={handleFormChange} placeholder="Thời lượng" />
        <input name="imageUrl" value={form.imageUrl} onChange={handleFormChange} placeholder="Ảnh" />
        <input name="detailUrl" value={form.detailUrl} onChange={handleFormChange} placeholder="Link chi tiết" />
        <button type="submit">Thêm mới</button>
      </form>
    </div>
  );
}

export default App;
