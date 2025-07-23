import React, { useState } from "react";

const TOUR_SITES = [
  { value: "vietravel", label: "Vietravel" },
  { value: "dulichviet", label: "Du Lịch Việt" },
  // Thêm các trang khác nếu muốn
];

function App() {
  const [site, setSite] = useState(TOUR_SITES[0].value);
  const [crawlTime, setCrawlTime] = useState("now");
  const [pages, setPages] = useState(1);

  // Dữ liệu tour đã crawl (mock)
  const [tours, setTours] = useState([]);

  const handleCrawl = () => {
    // Gọi API backend để crawl, cập nhật state tours
    alert(`Crawl ${pages} page(s) from ${site} at ${crawlTime}`);
  };

  return (
    <div style={{ maxWidth: 800, margin: "auto", padding: 24 }}>
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
        <label>Số page muốn crawl: </label>
        <input
          type="number"
          min={1}
          value={pages}
          onChange={e => setPages(e.target.value)}
          style={{ width: 60, marginLeft: 8 }}
        />
      </div>
      <button onClick={handleCrawl}>Bắt đầu crawl</button>

      <h2 style={{ marginTop: 32 }}>Danh sách tour đã crawl</h2>
      <table border="1" cellPadding={8} style={{ width: "100%", marginTop: 16 }}>
        <thead>
          <tr>
            <th>Tên tour</th>
            <th>Giá</th>
            <th>Ngày khởi hành</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {tours.length === 0 ? (
            <tr>
              <td colSpan={4} style={{ textAlign: "center" }}>Chưa có dữ liệu</td>
            </tr>
          ) : (
            tours.map((tour, idx) => (
              <tr key={idx}>
                <td>{tour.name}</td>
                <td>{tour.price}</td>
                <td>{tour.date}</td>
                <td>
                  <button>Sửa</button>
                  <button>Xóa</button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

export default App;
